using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization.Tables;
using Object = UnityEngine.Object;

namespace DeadAir.Editor
{
    public static class FirstNightV02Builder
    {
        public const string ScenePath = "Assets/_Game/Scenes/MotelFirstNight_V02.unity";
        private const string SourceScene = "Assets/_Game/Scenes/MotelArrival_Trial.unity";
        private const string Data = "Assets/_Game/Data/MotelFirstNightV02/";
        [Serializable] private sealed class Seed { public Row[] entries; }
        [Serializable] private sealed class Row { public string key, en, ru; }
        [MenuItem("89.4/First Night/Open V02 Empty Corridor Trial")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Build();
        }
        public static void Build()
        {
            InstallText();
            if (File.Exists(ScenePath)) { EditorSceneManager.OpenScene(ScenePath); return; }
            // One-time conversion of our preserved local v01 geometry; normal checkout ships v02 already wired.
            if (!File.Exists(SourceScene) || !AssetDatabase.CopyAsset(SourceScene, ScenePath))
                throw new IOException("Restore the delivered v02 scene, or provide the preserved v01 graybox for initial conversion.");
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var all = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MonoBehaviour>(true)).ToArray();
            var old = all.Single(component => component != null && component.GetType().Name == "MotelArrivalController");
            var oldData = new SerializedObject(old);
            var controller = old.gameObject.AddComponent<FirstNightController>(); controller.name = "FirstNightV02";
            foreach (string field in new[] { "reader", "player", "interactor", "radio", "battery", "reception", "guestRoom", "corridor", "personVisual", "location", "objective", "status", "controls", "prompt" })
                Assign(controller, field, oldData.FindProperty(field).objectReferenceValue);
            var radio = (RadioController)oldData.FindProperty("radio").objectReferenceValue;
            Assign(controller, "radioInput", radio.GetComponent<RadioInput>());
            radio.GetComponent<RadioInput>().enabled = false;
            var radioData = new SerializedObject(radio); radioData.FindProperty("startPoweredOn").boolValue = false;
            radioData.FindProperty("drainPerSecond").floatValue = 0; radioData.ApplyModifiedPropertiesWithoutUndo();
            var room = (GameObject)oldData.FindProperty("guestRoom").objectReferenceValue;
            Assign(controller, "roomReceiver", room.transform.Find("ReceiverReview").GetComponent<SpriteRenderer>());
            var map = new Dictionary<string, (FirstNightAction action, string prompt)>
            {
                ["CheckIn"] = (FirstNightAction.CheckIn, "checkin"), ["GuestDoor"] = (FirstNightAction.GuestDoor, "guestdoor"),
                ["Receiver"] = (FirstNightAction.RetrieveReceiver, "retrieve"), ["CorridorDoor"] = (FirstNightAction.CorridorDoor, "corridordoor"),
                ["ReturnToRoom"] = (FirstNightAction.ReturnToRoom, "return"), ["Person"] = (FirstNightAction.Man, "man"),
                ["TrialNotes"] = (FirstNightAction.LookAround, "look"), ["TestBattery"] = (FirstNightAction.TestBattery, "battery")
            };
            foreach (var component in all.Where(component => component != null && component.GetType().Name == "MotelArrivalInteractable"))
            {
                var action = new SerializedObject(component).FindProperty("action");
                var entry = map[action.enumNames[action.enumValueIndex]];
                AddInteraction(component.gameObject, entry.action, entry.prompt);
                Object.DestroyImmediate(component);
            }
            var bag = room.transform.Find("BagPlaceholder").gameObject;
            bag.AddComponent<BoxCollider2D>().isTrigger = true;
            AddInteraction(bag, FirstNightAction.ReviewReceiver, "review");
            Assign(controller, "checkIn", Document("CheckIn", "checkin.title", "checkin.01", "checkin.02", "checkin.03", "checkin.04", "checkin.05"));
            Assign(controller, "inspection", Document("Inspection", "radio.title", "radio.inspect"));
            Assign(controller, "retrieval", Document("Retrieval", "radio.title", "radio.take"));
            Assign(controller, "retrievalAfterClaim", Document("RetrievalAfterClaim", "radio.title", "radio.reason", "radio.take"));
            Assign(controller, "programBeforeClaim", Document("EarlyProgram", "radio.title", "program.text"));
            Assign(controller, "program", Document("Program", "radio.title", "program.text", "program.observation"));
            Assign(controller, "introduction", Document("Introduction", "intro.title", "intro.01", "intro.02", "intro.03", "intro.04"));
            Assign(controller, "emptyCorridor", Document("EmptyCorridor", "empty.title", "empty.01", "empty.02", "end.notice"));
            Assign(controller, "needCheckIn", Document("NeedCheckIn", "blocked.title", "blocked.checkin"));
            Assign(controller, "needPower", Document("NeedPower", "radio.title", "radio.power_hint"));
            // Give the status line its own floor strip rather than crossing the actor's feet.
            var status = (TMPro.TMP_Text)oldData.FindProperty("status").objectReferenceValue;
            status.rectTransform.anchoredPosition = new Vector2(20, -650); status.rectTransform.sizeDelta = new Vector2(1240, 55);
            Object.DestroyImmediate(old);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            Debug.Log("Separate first-night v02 created: packed/OFF, explicit retrieval, ordinary offscreen departure; no route/startup changes.");

            void AddInteraction(GameObject go, FirstNightAction action, string prompt)
            {
                var component = go.AddComponent<FirstNightInteractable>(); Assign(component, "controller", controller);
                var data = new SerializedObject(component); data.FindProperty("action").enumValueIndex = (int)action;
                data.FindProperty("promptKey").stringValue = "first_night.prompt." + prompt; data.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        public static void InstallText()
        {
            AssetDatabase.Refresh();
            var seed = JsonUtility.FromJson<Seed>(File.ReadAllText(Data + "FirstNightSeed.json"));
            var collection = LocalizationEditorSettings.GetStringTableCollection(PrototypeText.TableName);
            foreach (string locale in new[] { "en", "ru" })
            {
                var table = (StringTable)collection.GetTable(locale);
                foreach (var row in seed.entries)
                    if (table.GetEntry(row.key) == null) table.AddEntry(row.key, locale == "en" ? row.en : row.ru);
                EditorUtility.SetDirty(table);
            }
            EditorUtility.SetDirty(collection.SharedData); AssetDatabase.SaveAssets();
        }
        private static EvidenceDefinition Document(string name, string title, params string[] keys)
        {
            string path = Data + name + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<EvidenceDefinition>(); AssetDatabase.CreateAsset(asset, path);
            var data = new SerializedObject(asset); data.FindProperty("evidenceId").stringValue = "first_night." + name;
            data.FindProperty("title").stringValue = "first_night." + title;
            var pages = data.FindProperty("pages"); pages.arraySize = keys.Length;
            for (int i = 0; i < keys.Length; i++) pages.GetArrayElementAtIndex(i).stringValue = "first_night." + keys[i];
            data.ApplyModifiedPropertiesWithoutUndo(); return asset;
        }
        private static void Assign(Object target, string field, Object value)
        {
            var data = new SerializedObject(target); data.FindProperty(field).objectReferenceValue = value; data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
