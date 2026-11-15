using System;
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
    public static class CurrentPlayableInstaller
    {
        public const string Entry = "Assets/_Game/Scenes/CurrentPrologue.unity";
        public const string Motel = "Assets/_Game/Scenes/CurrentMotel.unity";
        private const string Art = "Assets/_Game/Art/CurrentPlayable/";
        private const string Data = "Assets/_Game/Data/CurrentPlayable/";
        private const string Audio = "Assets/_Game/Audio/CurrentFirstNight/";
        [Serializable] private sealed class Manifest { public View[] views; }
        [Serializable] private sealed class View { public string name; public Layer[] layers; }
        [Serializable] private sealed class Layer
        {
            public string asset; public float[] rect, unityPosition, pivotOnFullPNG, scale;
            public float pixelsPerUnit; public int sortingOrder;
        }
        private static Manifest manifest;

        [MenuItem("89.4/Play Current Game")]
        public static void Play()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            CreateCurrentPlayable(); EditorApplication.isPlaying = true;
        }
        public static void CreateCurrentPlayable()
        {
            if (Ready(Entry) && Ready(Motel)) { EditorSceneManager.OpenScene(Entry); return; }
            Directory.CreateDirectory(Data); AssetDatabase.Refresh();
            manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(Art + "floor_manifest.json"));
            foreach (var layer in manifest.views.SelectMany(view => view.layers).GroupBy(layer => layer.asset).Select(group => group.First()))
                Import(layer.asset, layer.pixelsPerUnit, new Vector2(layer.pivotOnFullPNG[0], layer.pivotOnFullPNG[1]));
            Import("reception_plate", 941f / 4.5f, new Vector2(.5f, 0));
            foreach (string path in Directory.GetFiles(Audio, "*.wav"))
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
                var settings = importer.defaultSampleSettings; settings.compressionFormat = AudioCompressionFormat.PCM;
                settings.loadType = AudioClipLoadType.DecompressOnLoad; settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                importer.defaultSampleSettings = settings; importer.forceToMono = false; importer.SaveAndReimport();
            }
            AddText();
            if (!Ready(Motel))
            {
                if (!File.Exists(Motel) && !AssetDatabase.CopyAsset(FirstNightV02Builder.ScenePath, Motel)) throw new IOException("Could not copy v02 to current motel.");
                var scene = EditorSceneManager.OpenScene(Motel);
                var controller = Find<FirstNightController>(); var data = new SerializedObject(controller);
                var reception = (GameObject)data.FindProperty("reception").objectReferenceValue;
                var room = (GameObject)data.FindProperty("guestRoom").objectReferenceValue;
                var corridor = (GameObject)data.FindProperty("corridor").objectReferenceValue;
                foreach (var space in new[] { reception, room, corridor })
                {
                    foreach (var renderer in space.GetComponentsInChildren<SpriteRenderer>(true)) renderer.enabled = false;
                    Floor(space.transform); foreach (var test in space.GetComponentsInChildren<FirstNightInteractable>(true).Where(item => item.name == "TEST_BATTERY"))
                    { test.transform.position = new Vector3(7.4f, .9f, 0); test.gameObject.SetActive(false); }
                }
                Background("reception_plate", reception.transform);
                foreach (var layer in ViewOf("guest_receiver_left").layers.Where(layer => layer.asset == "guest_room_base")) Sprite(layer, room.transform);
                var bag = Paint(room.transform.Find("BagPlaceholder").gameObject, LayerOf("guest_receiver_left", "travel_bag"));
                var receiver = Paint(room.transform.Find("ReceiverReview").gameObject, LayerOf("guest_receiver_left", "radio_portable"));
                Assign(controller, "roomReceiver", receiver);
                Box(reception.transform.Find("CurrentRegistration").gameObject, new Vector2(6.2f, 1.8f), new Vector2(.9f, .5f));
                Box(reception.transform.Find("GuestRoomDoor").gameObject, new Vector2(1.8f, 2), new Vector2(1.1f, 2.4f));
                Box(room.transform.Find("CorridorDoor").gameObject, new Vector2(1.5f, 2), new Vector2(.95f, 2.4f));
                foreach (var layer in ViewOf("corridor_first_meeting").layers.Where(layer => layer.asset is "corridor_base" or "door_closed" or "lamp_on")) Sprite(layer, corridor.transform);
                Box(corridor.transform.Find("OwnRoomDoor").gameObject, new Vector2(1.9167f, 2.2875f), new Vector2(1.23f, 2.5f));
                var person = corridor.transform.Find("Person"); person.position = new Vector3(5.08333f, .5f, 0); person.localScale = Vector3.one;
                var visual = (GameObject)data.FindProperty("personVisual").objectReferenceValue;
                foreach (Transform child in visual.transform.Cast<Transform>().ToArray()) Object.DestroyImmediate(child.gameObject);
                visual.transform.localPosition = Vector3.zero; visual.transform.localScale = Vector3.one;
                var witness = LayerOf("corridor_first_meeting", "witness_idle_left");
                var human = Sprite(witness, visual.transform); human.transform.localPosition = Vector3.zero;
                var personBox = person.GetComponent<BoxCollider2D>(); personBox.size = new Vector2(.523f, 1.85f); personBox.offset = new Vector2(0, .925f);
                var shadow = Sprite(LayerOf("home_phone", "contact_shadow"), visual.transform); shadow.transform.localPosition = new Vector3(0, -.0375f, 0);
                Box(corridor.transform.Find("TrialNotice").gameObject, new Vector2(4.5f, 1.3f), new Vector2(.8f, 1.6f));
                // No invisible prompt before this authored empty-corridor look-around becomes valid.
                var movement = Find<PlayerController2D>(); Actor(movement.gameObject, "hero_idle_right", controller);
                Set(controller, "standingFeetY", .5f); Set(controller, "receptionSpawnX", 5.65f);
                Set(controller, "roomEntryX", 2.25f); Set(controller, "roomReturnX", 2.25f); Set(controller, "corridorEntryX", 2.75f);
                SetString(controller, "productionCaptionKey", "current.audio.caption");
                var interaction = movement.GetComponent<InteractionController>(); Set(interaction, "radius", .8f);
                CameraAndUi(); AddAudio(controller);
                new GameObject("CurrentPlayable_PresentationAssigned"); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            }
            if (!Ready(Entry))
            {
                if (!File.Exists(Entry) && !AssetDatabase.CopyAsset("Assets/_Game/Scenes/PrologueAssignment_ArtTrial04.unity", Entry)) throw new IOException("Existing painted prologue required.");
                var scene = EditorSceneManager.OpenScene(Entry);
                foreach (var root in scene.GetRootGameObjects().Where(root => root.name.StartsWith("WorkingRoomPlate"))) Object.DestroyImmediate(root);
                var room = scene.GetRootGameObjects().Single(root => root.name == "OrdinaryRoom");
                foreach (var renderer in room.GetComponentsInChildren<SpriteRenderer>(true)) renderer.enabled = false;
                Floor(room.transform);
                Sprite(LayerOf("home_phone", "home_base"), room.transform);
                Paint(room.transform.Find("Telephone").gameObject, LayerOf("home_phone", "telephone"));
                Paint(room.transform.Find("DeskRadio").gameObject, LayerOf("home_phone", "radio_portable"));
                Paint(room.transform.Find("CaseFolder").gameObject, LayerOf("home_phone", "envelope"));
                Paint(room.transform.Find("TravelBag").gameObject, LayerOf("home_phone", "travel_bag"));
                Box(room.transform.Find("Exit").gameObject, new Vector2(6.3167f, 2), new Vector2(1.1f, 2.5f));
                var movement = Find<PlayerController2D>(); Actor(movement.gameObject, "hero_home_idle_right", null);
                movement.transform.position = new Vector3(2.15f, 1.43f, 0); Set(movement.GetComponent<InteractionController>(), "radius", .8f);
                var controller = Find<PrologueController>();
                const string currentDefinition = Data + "CurrentPrologue.asset";
                if (!File.Exists(currentDefinition)) AssetDatabase.CopyAsset("Assets/_Game/Data/Prologue/Prologue.asset", currentDefinition);
                var definition = AssetDatabase.LoadAssetAtPath<PrologueDefinition>(currentDefinition);
                SetString(definition, "targetScenePath", Motel); Bool(definition, "playableCheckInArrival", true);
                Assign(controller, "definition", definition); Assign(controller.Reader, "definition", definition); Bool(controller, "tintPackedBag", false);
                var audio = Find<PrologueAudio>(); Assign(audio, "definition", definition);
                var audioData = new SerializedObject(audio);
                foreach (string field in new[] { "ambience", "broadcast", "ring", "receiver" })
                    ((AudioSource)audioData.FindProperty(field).objectReferenceValue).volume = .8f;
                const string journey = "Assets/_Game/Prefabs/CurrentPlayable/CurrentJourney.prefab";
                Directory.CreateDirectory("Assets/_Game/Prefabs/CurrentPlayable"); AssetDatabase.Refresh();
                if (!File.Exists(journey)) AssetDatabase.CopyAsset("Assets/_Game/Prefabs/Prologue/Journey.prefab", journey);
                Assign(controller, "journeyPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(journey));
                CameraAndUi(); new GameObject("CurrentPlayable_PresentationAssigned"); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            }
            var previous = EditorBuildSettings.scenes.Where(entry => entry.path != Entry && entry.path != Motel).ToArray();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Entry, true), new EditorBuildSettingsScene(Motel, true) }.Concat(previous).ToArray();
            AssetDatabase.SaveAssets(); EditorSceneManager.OpenScene(Entry);
            Debug.Log("Current playable connected: painted prologue -> OFF reception -> first night, with assigned art/audio. Historical scenes retained.");
        }
        private static bool Ready(string path) => File.Exists(path) && File.ReadAllText(path).Contains("CurrentPlayable_PresentationAssigned");
        private static T Find<T>() where T : Object => Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        private static View ViewOf(string name) => manifest.views.Single(view => view.name == name);
        private static Layer LayerOf(string view, string asset) => ViewOf(view).layers.First(layer => layer.asset == asset);
        private static void Import(string asset, float ppu, Vector2 pivot)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Art + asset + ".png");
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single; importer.spritePixelsPerUnit = ppu;
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect; settings.spriteAlignment = (int)SpriteAlignment.Custom; settings.spritePivot = pivot;
            importer.SetTextureSettings(settings); importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096; importer.alphaIsTransparency = true; importer.SaveAndReimport();
        }
        private static SpriteRenderer Sprite(Layer layer, Transform parent)
        {
            var go = new GameObject(layer.asset); go.transform.SetParent(parent, false); return Paint(go, layer);
        }
        private static SpriteRenderer Paint(GameObject go, Layer layer)
        {
            var sprite = go.GetComponent<SpriteRenderer>(); if (sprite == null) sprite = go.AddComponent<SpriteRenderer>();
            sprite.enabled = true; sprite.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + layer.asset + ".png"); sprite.color = Color.white;
            sprite.sortingOrder = layer.sortingOrder; sprite.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            go.transform.position = new Vector3(layer.unityPosition[0], layer.unityPosition[1], 0);
            go.transform.localScale = new Vector3(layer.scale[0], layer.scale[1], 1);
            if (go.TryGetComponent<BoxCollider2D>(out var box)) { box.size = sprite.sprite.bounds.size; box.offset = sprite.sprite.bounds.center; }
            return sprite;
        }
        private static void Background(string name, Transform parent)
        {
            var layer = new Layer { asset = name, unityPosition = new[] { 4f, 0f }, scale = new[] { 8f / (1672f / (941f / 4.5f)), 1f }, sortingOrder = -10 };
            Sprite(layer, parent);
        }
        private static void Box(GameObject go, Vector2 position, Vector2 size)
        {
            go.transform.position = position; go.transform.localScale = Vector3.one;
            var box = go.GetComponent<BoxCollider2D>(); box.offset = Vector2.zero; box.size = size;
        }
        private static void Floor(Transform root)
        {
            root.Find("Floor").position = new Vector3(4, 0, 0); root.Find("Floor").localScale = new Vector3(9, 1, 1);
            root.Find("LeftWall").position = new Vector3(-.5f, 2.5f, 0); root.Find("LeftWall").localScale = new Vector3(1, 7, 1);
            root.Find("RightWall").position = new Vector3(8.5f, 2.5f, 0); root.Find("RightWall").localScale = new Vector3(1, 7, 1);
        }
        private static void Actor(GameObject player, string asset, FirstNightController firstNight)
        {
            player.GetComponent<SpriteRenderer>().enabled = false;
            if (player.transform.Find("ArtVisuals") != null) Object.DestroyImmediate(player.transform.Find("ArtVisuals").gameObject);
            var oldFacing = player.GetComponents<MonoBehaviour>().FirstOrDefault(component => component.GetType().Name == "MotelActorFacing2D"); if (oldFacing != null) Object.DestroyImmediate(oldFacing);
            var root = new GameObject("ArtVisuals"); root.transform.SetParent(player.transform, false);
            root.transform.localPosition = new Vector3(0, -.5f, 0);
            root.transform.localScale = new Vector3(1 / player.transform.localScale.x, 1 / player.transform.localScale.y, 1);
            var face = new GameObject("Facing"); face.transform.SetParent(root.transform, false);
            var layer = manifest.views.SelectMany(view => view.layers).First(layer => layer.asset == asset);
            var hero = Sprite(layer, face.transform); hero.transform.localPosition = Vector3.zero;
            var shadow = Sprite(LayerOf("home_phone", "contact_shadow"), root.transform); shadow.transform.localPosition = new Vector3(0, -.0375f, 0);
            GameObject carried = null;
            if (firstNight != null)
            {
                carried = Sprite(LayerOf("guest_receiver_carried", "radio_portable"), face.transform).gameObject;
                carried.transform.localPosition = new Vector3(.27083f, 1.06667f, 0); carried.GetComponent<SpriteRenderer>().sortingOrder = 11; carried.SetActive(false);
            }
            var visuals = player.AddComponent<CurrentActorVisuals>(); Assign(visuals, "body", player.GetComponent<Rigidbody2D>());
            Assign(visuals, "facingRoot", face.transform); Assign(visuals, "firstNight", firstNight); Assign(visuals, "carriedReceiver", carried);
        }
        private static void CameraAndUi()
        {
            var camera = Find<Camera>(); camera.orthographicSize = 2.25f; camera.transform.position = new Vector3(4, 2.25f, -10);
            foreach (var text in Object.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (text.name == "Status") { text.rectTransform.anchoredPosition = new Vector2(20, -665); text.rectTransform.sizeDelta = new Vector2(1240, 45); }
            HudBacking();
        }
        public static void ApplyReadableHud()
        {
            foreach (string path in new[] { Entry, Motel })
            {
                var scene = EditorSceneManager.OpenScene(path); HudBacking();
                EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            }
            EditorSceneManager.OpenScene(Entry);
        }
        private static void HudBacking()
        {
            var canvas = Find<Canvas>();
            if (canvas.transform.Find("HUD_ReadabilityBacking") != null) return;
            var go = new GameObject("HUD_ReadabilityBacking", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            go.transform.SetParent(canvas.transform, false); go.transform.SetAsFirstSibling();
            var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(1280, 210);
            var image = go.GetComponent<UnityEngine.UI.Image>(); image.color = new Color(0, 0, 0, .38f); image.raycastTarget = false;
        }
        private static void AddAudio(FirstNightController controller)
        {
            var audio = controller.gameObject.AddComponent<FirstNightAudio>();
            Assign(audio, "controller", controller); Assign(audio, "reader", controller.Reader);
            Assign(audio, "radio", Find<RadioController>()); Assign(audio, "body", Find<PlayerController2D>().GetComponent<Rigidbody2D>());
            Source("ambience", "amb_motel_guest_room_loop", true, 1);
            Source("texture", "radio_motel_receiver_texture_loop", true, 1);
            Source("voice", "radio_motel_announcement_WORKING_TTS", false, 1);
            foreach (string field in new[] { "switches", "door", "handling", "outsideMovement", "footsteps" }) Source(field, null, false, field == "footsteps" ? .4f : .8f);
            foreach (var pair in new[] { ("onClip", "sfx_radio_on"), ("offClip", "sfx_radio_off"), ("doorClip", "sfx_guestroom_door_soft_01"), ("retrieveClip", "sfx_radio_retrieve_soft_01"), ("clothClip", "sfx_corridor_cloth_soft_01"), ("stepClip", "sfx_corridor_step_soft_01") })
                Assign(audio, pair.Item1, AssetDatabase.LoadAssetAtPath<AudioClip>(Audio + pair.Item2 + ".wav"));
            var data = new SerializedObject(audio); var programs = data.FindProperty("programDocuments"); programs.arraySize = 2;
            for (int i = 0; i < 2; i++) programs.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>("Assets/_Game/Data/MotelFirstNightV02/" + (i == 0 ? "Program" : "EarlyProgram") + ".asset");
            data.ApplyModifiedPropertiesWithoutUndo();
            void Source(string field, string clip, bool loop, float volume)
            {
                var go = new GameObject("Audio_" + field); go.transform.SetParent(controller.transform, false);
                var source = go.AddComponent<AudioSource>(); source.playOnAwake = false; source.loop = loop; source.spatialBlend = 0; source.volume = volume;
                if (clip != null) source.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Audio + clip + ".wav"); Assign(audio, field, source);
            }
        }
        private static void AddText()
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(PrototypeText.TableName);
            foreach (string locale in new[] { "en", "ru" })
            {
                var table = (StringTable)collection.GetTable(locale);
                table.AddEntry("current.audio.caption", locale == "en" ? "WORKING audio: announcement and receiver texture; music break pending." : "Рабочий эфир: текст и шум приёмника. Музыкальная пауза пока не создана.");
                EditorUtility.SetDirty(table);
            }
            EditorUtility.SetDirty(collection.SharedData);
        }
        private static void Assign(Object target, string field, Object value)
        { var data = new SerializedObject(target); data.FindProperty(field).objectReferenceValue = value; data.ApplyModifiedPropertiesWithoutUndo(); }
        private static void Set(Object target, string field, float value)
        { var data = new SerializedObject(target); data.FindProperty(field).floatValue = value; data.ApplyModifiedPropertiesWithoutUndo(); }
        private static void Bool(Object target, string field, bool value)
        { var data = new SerializedObject(target); data.FindProperty(field).boolValue = value; data.ApplyModifiedPropertiesWithoutUndo(); }
        private static void SetString(Object target, string field, string value)
        { var data = new SerializedObject(target); data.FindProperty(field).stringValue = value; data.ApplyModifiedPropertiesWithoutUndo(); }
    }
}
