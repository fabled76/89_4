using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

namespace DeadAir.Editor
{
    public static class FirstWitnessSceneInstaller
    {
        private const string DataFolder = "Assets/_Game/Data/FirstWitness";

        [MenuItem("89.4/Add First Witness to Existing Corridor")]
        public static void InstallFromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Exit Play mode", "Install the encounter outside Play mode.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Install();
        }

        // Additive content migration: opens and saves the EXISTING scene, never creates a replacement.
        public static void Install()
        {
            if (!File.Exists(MotelPrototypeBuilder.ScenePath)) throw new FileNotFoundException("Existing corridor required.");
            var scene = SceneManager.GetSceneByPath(MotelPrototypeBuilder.ScenePath);
            if (!scene.IsValid() || !scene.isLoaded) scene = EditorSceneManager.OpenScene(MotelPrototypeBuilder.ScenePath);
            var roots = scene.GetRootGameObjects();
            if (Array.Exists(roots, obj => obj.name == "FirstWitness"))
            {
                Debug.Log("First Witness already installed; existing content and tuning were left intact.");
                return;
            }
            var player = RequiredRoot(roots, "Player");
            var hud = RequiredRoot(roots, "PrototypeHUD");
            var environment = RequiredRoot(roots, "Environment");
            var silhouette = environment.transform.Find("DRIFT_Visuals/Silhouette_VisualOnly_NoAI");
            if (silhouette == null) throw new InvalidOperationException("Existing DRIFT silhouette required.");
            var sprite = silhouette.GetComponent<SpriteRenderer>().sprite;
            var radio = player.GetComponentInChildren<RadioController>();
            var battery = player.GetComponentInChildren<BatteryComponent>();
            if (radio == null || battery == null) throw new InvalidOperationException("Existing portable radio required.");

            Directory.CreateDirectory(DataFolder);
            AssetDatabase.Refresh();
            var ledger = Evidence("MotelLedger", "motel.guest-ledger", "evidence.ledger.title", "evidence.ledger.page.01", "evidence.ledger.page.02", "evidence.ledger.page.03");
            var photo = Evidence("MotelPhoto", "motel.corridor-photo", "evidence.photo.title", "evidence.photo.page.01", "evidence.photo.page.02");
            var testimony = Evidence("WitnessTestimony", "witness.station-lead", "evidence.testimony.title", "evidence.testimony.page.01", "evidence.testimony.page.02", "evidence.testimony.page.03", "evidence.testimony.page.04", "evidence.testimony.page.05");

            var scenario = Asset<FirstWitnessDefinition>("FirstWitnessScenario");
            SetText(scenario, "encounterTitle", "witness.encounterTitle");
            SetText(scenario, "emptyJournalMessage", "witness.emptyJournalMessage");
            Assign(scenario, "ledger", ledger); Assign(scenario, "photo", photo); Assign(scenario, "testimony", testimony);
            SetText(scenario, "exploreObjective", "witness.exploreObjective");
            SetText(scenario, "figureObjective", "witness.figureObjective");
            SetText(scenario, "ledgerObjective", "witness.ledgerObjective");
            SetText(scenario, "returnObjective", "witness.returnObjective");
            SetText(scenario, "approachObjective", "witness.approachObjective");
            SetText(scenario, "completedObjective", "witness.completedObjective");

            var root = new GameObject("FirstWitness");
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.position = new Vector3(silhouette.position.x, 1.2f, 0);
            var journal = root.AddComponent<EvidenceJournal>();
            var tracker = root.AddComponent<InvestigationStateTracker>();
            Assign(tracker, "journal", journal); Assign(tracker, "scenario", scenario);

            var objective = Text("InvestigationObjective", hud.transform, new Vector2(20, -115), new Vector2(1230, 100), 23);
            var oldPrompt = hud.transform.Find("InteractionPrompt") as RectTransform;
            if (oldPrompt != null) oldPrompt.anchoredPosition = new Vector2(20, -225);
            var panel = new GameObject("InvestigationPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(hud.transform, false);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = panelRect.anchorMax = new Vector2(.5f, .5f);
            panelRect.sizeDelta = new Vector2(1160, 400);
            panelRect.anchoredPosition = new Vector2(0, -80);
            panel.GetComponent<Image>().color = new Color(.025f, .035f, .05f, .98f);
            panel.GetComponent<Image>().raycastTarget = false;
            var heading = Text("Heading", panel.transform, new Vector2(25, -18), new Vector2(1110, 45), 28);
            var body = Text("Body", panel.transform, new Vector2(25, -75), new Vector2(1110, 255), 25);
            var footer = Text("Footer", panel.transform, new Vector2(25, -352), new Vector2(1110, 40), 19);
            var ui = root.AddComponent<InvestigationUI>();
            Assign(ui, "journal", journal); Assign(ui, "investigation", tracker);
            Assign(ui, "movement", player.GetComponent<PlayerController2D>());
            Assign(ui, "interactor", player.GetComponent<InteractionController>());
            Assign(ui, "panel", panel); Assign(ui, "headingText", heading); Assign(ui, "bodyText", body);
            Assign(ui, "footerText", footer); Assign(ui, "objectiveText", objective);
            panel.SetActive(false);

            var trigger = root.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(3.2f, 3f);
            var encounter = root.AddComponent<UnacceptedEncounter>();
            Assign(encounter, "radio", radio); Assign(encounter, "battery", battery);
            Assign(encounter, "investigation", tracker); Assign(encounter, "journal", journal);
            Assign(encounter, "scenario", scenario); Assign(encounter, "presentation", ui);
            Assign(encounter, "playerBody", player.GetComponent<Rigidbody2D>());
            Clue("MotelGuestLedger", root.transform, sprite, new Vector2(-5, .65f), new Vector2(1, 1.3f), new Color(.85f, .8f, .58f), ledger, journal, ui, "prompt.ledger");
            Clue("MotelCorridorPhoto", root.transform, sprite, new Vector2(-13, 1f), new Vector2(.9f, 1.5f), new Color(.67f, .65f, .57f), photo, journal, ui, "prompt.photo");

            // Tune this scene only: same drain rule, more time for reading and exploration.
            var radioData = new SerializedObject(radio);
            radioData.FindProperty("drainPerSecond").floatValue = .2f;
            radioData.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Could not save upgraded corridor.");
            AssetDatabase.SaveAssets();
            Debug.Log("Added The First Witness to the existing corridor; original scene GUID and objects preserved.");
        }

        private static GameObject RequiredRoot(GameObject[] roots, string name)
            => Array.Find(roots, root => root.name == name) ?? throw new InvalidOperationException("Missing scene root: " + name);

        private static T Asset<T>(string name) where T : ScriptableObject
        {
            string path = DataFolder + "/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            var value = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(value, path);
            return value;
        }

        private static EvidenceDefinition Evidence(string name, string id, string title, params string[] pages)
        {
            string path = DataFolder + "/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>(path);
            if (existing != null) return existing;
            var evidence = Asset<EvidenceDefinition>(name);
            SetText(evidence, "evidenceId", id); SetText(evidence, "title", title);
            var data = new SerializedObject(evidence);
            var array = data.FindProperty("pages");
            array.arraySize = pages.Length;
            for (int i = 0; i < pages.Length; i++) array.GetArrayElementAtIndex(i).stringValue = pages[i];
            data.ApplyModifiedPropertiesWithoutUndo();
            return evidence;
        }

        private static void Clue(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size,
            Color color, EvidenceDefinition evidence, EvidenceJournal journal, InvestigationUI ui, string prompt)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 1;
            renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            var clue = obj.AddComponent<EvidenceInteractable>();
            Assign(clue, "evidence", evidence); Assign(clue, "journal", journal); Assign(clue, "presentation", ui);
            SetText(clue, "prompt", prompt);
        }

        private static TMP_Text Text(string name, Transform parent, Vector2 position, Vector2 size, int fontSize)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var text = obj.GetComponent<TextMeshProUGUI>();
            text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LocalizationSceneInstaller.FontPath);
            text.fontSize = fontSize;
            text.color = new Color(.93f, .92f, .85f);
            text.raycastTarget = false;
            text.richText = false;
            return text;
        }
        private static void Assign(Object target, string field, Object value)
        {
            var data = new SerializedObject(target);
            data.FindProperty(field).objectReferenceValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void SetText(Object target, string field, string value)
        {
            var data = new SerializedObject(target);
            data.FindProperty(field).stringValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
