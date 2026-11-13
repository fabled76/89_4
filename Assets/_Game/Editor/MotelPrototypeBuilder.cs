using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeadAir.Editor
{
    public static class MotelPrototypeBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/MotelCorridor.unity";
        private const string SpritePath = "Assets/_Game/Art/Placeholders/WhiteSquare.png";

        [MenuItem("89.4/Create Motel Corridor Prototype")]
        public static void CreateFromMenu()
        {
            if (File.Exists(ScenePath))
            {
                EditorUtility.DisplayDialog("Scene already exists", "Open " + ScenePath + ". The builder will not overwrite your scene.", "OK");
                return;
            }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) Build();
        }

        // May also be invoked with -executeMethod DeadAir.Editor.MotelPrototypeBuilder.Build.
        public static void Build()
        {
            if (File.Exists(ScenePath)) throw new IOException("Refusing to overwrite " + ScenePath);
            foreach (string folder in new[] { "Scenes", "Prefabs", "Art/Placeholders", "Audio", "Data", "UI" })
                Directory.CreateDirectory("Assets/_Game/" + folder);
            AssetDatabase.Refresh();
            var sprite = CreateSquare();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var environment = new GameObject("Environment");
            var shared = Child("SharedCollision", environment.transform);
            Rect("Floor", shared.transform, sprite, new Vector2(0, -0.5f), new Vector2(30, 1), new Color(.14f, .12f, .14f), 0, true);
            Rect("LeftWall", shared.transform, sprite, new Vector2(-15.5f, 3), new Vector2(1, 8), Color.gray, 0, true);
            Rect("RightWall", shared.transform, sprite, new Vector2(15.5f, 3), new Vector2(1, 8), Color.gray, 0, true);
            Rect("Ceiling", shared.transform, sprite, new Vector2(0, 7.5f), new Vector2(30, 1), Color.gray, 0, true);
            var stable = Child("STABLE_Visuals", environment.transform);
            var drift = Child("DRIFT_Visuals", environment.transform);
            CreatePresentation(stable.transform, sprite, new Color(.28f, .24f, .20f), new Color(.46f, .32f, .22f));
            CreatePresentation(drift.transform, sprite, new Color(.12f, .17f, .23f), new Color(.22f, .28f, .34f));
            Rect("Silhouette_VisualOnly_NoAI", drift.transform, sprite, new Vector2(7, .95f), new Vector2(.65f, 1.9f), new Color(.025f, .03f, .045f), 1);

            var player = Rect("Player", null, sprite, new Vector2(-10, 1), new Vector2(.7f, 1.8f), new Color(.8f, .65f, .36f), 2);
            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 3f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var collider = player.AddComponent<BoxCollider2D>();
            var material = new PhysicsMaterial2D("CorridorPlayer") { friction = 0f, bounciness = 0f };
            const string materialPath = "Assets/_Game/Data/CorridorPlayer.physicsMaterial2D";
            if (!File.Exists(materialPath)) AssetDatabase.CreateAsset(material, materialPath);
            else Object.DestroyImmediate(material);
            collider.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(materialPath);
            player.AddComponent<PlayerController2D>();
            var interactor = player.AddComponent<InteractionController>();
            var equipment = Child("PortableRadio", player.transform);
            var battery = equipment.AddComponent<BatteryComponent>();
            var radio = equipment.AddComponent<RadioController>();
            equipment.AddComponent<RadioInput>();

            var station = Rect("TestBatteryStation_Unlimited", null, sprite, new Vector2(-8, .65f), new Vector2(.8f, 1.3f), new Color(.35f, .65f, .4f), 1);
            station.AddComponent<BoxCollider2D>().isTrigger = true;
            Assign(station.AddComponent<BatteryReplacementInteractable>(), "battery", battery);
            var marker = Rect("TestInspectionMarker", null, sprite, new Vector2(1, .7f), new Vector2(.6f, 1.4f), new Color(.55f, .55f, .7f), 1);
            marker.AddComponent<BoxCollider2D>().isTrigger = true;
            marker.AddComponent<InspectInteractable>();

            var world = new GameObject("LocalCorridorPresentation").AddComponent<WorldStateController>();
            Assign(world, "radio", radio);
            Assign(world, "stableVisuals", stable);
            Assign(world, "driftVisuals", drift);
            drift.SetActive(false);

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-7, 2.5f, -10);
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 4.5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.04f, .05f, .07f);
            Assign(cameraObject.AddComponent<CameraFollow2D>(), "target", player.transform);

            var canvasObject = new GameObject("PrototypeHUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            var status = CreateText("RadioAndControls", canvasObject.transform, new Vector2(20, -20), new Vector2(1150, 130));
            var prompt = CreateText("InteractionPrompt", canvasObject.transform, new Vector2(20, -160), new Vector2(1150, 60));
            var hud = canvasObject.AddComponent<PrototypeHUD>();
            Assign(hud, "radio", radio);
            Assign(hud, "battery", battery);
            Assign(hud, "interactor", interactor);
            Assign(hud, "statusText", status);
            Assign(hud, "promptText", prompt);

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new IOException("Could not save corridor scene.");
            AssetDatabase.SaveAssets();
            Debug.Log("Created and wired " + ScenePath + ". Open it and press Play; see Docs/PROTOTYPE_SETUP.md.");
        }

        private static Sprite CreateSquare()
        {
            if (!File.Exists(SpritePath))
            {
                var texture = new Texture2D(8, 8);
                var pixels = new Color[64];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                texture.SetPixels(pixels);
                texture.Apply();
                File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(SpritePath);
            }
            var importer = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 8;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        }

        private static GameObject Child(string name, Transform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static GameObject Rect(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size, Color color, int order, bool solid = false)
        {
            var obj = Child(name, parent);
            obj.transform.localPosition = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = order;
            renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            if (solid) obj.AddComponent<BoxCollider2D>();
            return obj;
        }

        private static void CreatePresentation(Transform parent, Sprite sprite, Color wallColor, Color doorColor)
        {
            Rect("Background", parent, sprite, new Vector2(0, 3.5f), new Vector2(30, 7), wallColor, -10);
            for (int x = -12; x <= 12; x += 4)
            {
                Rect("Door_" + x, parent, sprite, new Vector2(x, 1.6f), new Vector2(1.5f, 3.2f), doorColor, -5);
                Rect("Lamp_" + x, parent, sprite, new Vector2(x, 4.8f), new Vector2(1.2f, .15f), doorColor * 1.5f, -4);
            }
        }

        private static Text CreateText(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var text = obj.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static void Assign(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
