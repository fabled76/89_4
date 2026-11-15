using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Editor.Studies
{
    public static class ArtPaletteCV02Installer
    {
        public const string Data = "Assets/_Game/Data/Studies/PaletteC_v02/";
        public const string Art = "Assets/_Game/Art/Studies/PaletteC_v02/";
        [Serializable] private sealed class Manifest { public string deliveryVersion; public Layer[] layers; }
        [Serializable] private sealed class Layer
        {
            public string id, sha256;
            public int pixelsPerUnit, sortingOrder;
            public float forwardDepthOffsetMeters;
            public bool followReferenceRootHorizontalDelta;
        }

        // Explicit versioned integration, never a latest-version directory watcher.
        public static void Install()
        {
            const string manifestHash = "d12e12bafa4cf1c65772ed857fbe5fba4c7af8bd0faf3b9823caca519451b0dd";
            if (Hash(Data + "manifest.json") != manifestHash) throw new InvalidDataException("Pinned v02 manifest changed.");
            var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(Data + "manifest.json"));
            if (manifest.layers.Length != 5) throw new InvalidDataException("Exactly five v02 layers expected.");
            foreach (var layer in manifest.layers)
                if (Hash(Art + layer.id + ".png") != layer.sha256) throw new InvalidDataException("Layer mismatch: " + layer.id);
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(ArtPaletteCPilotSetup.ScenePath);
            if (GameObject.Find("PaletteC_v02_Integrated") != null) return; // Preserve manual pilot changes.
            if (scene.GetRootGameObjects().Any(root => root.name.StartsWith("PaletteC_") && root.name.EndsWith("_Integrated")))
                throw new InvalidOperationException("Another art revision is already assigned. Use an explicit version refresh, never rebuild over it.");
            var camera = Camera.main;
            var actor = UnityEngine.Object.FindFirstObjectByType<PlayerController2D>();
            foreach (var renderer in GameObject.Find("MeasuredGeometry").GetComponentsInChildren<MeshRenderer>(true)) renderer.enabled = false;
            var root = new GameObject("PaletteC_v02_Integrated");
            var moving = new GameObject("FollowingReferenceAndContact"); moving.transform.SetParent(root.transform);
            moving.transform.SetPositionAndRotation(new Vector3(0, 1.2f, .25f), camera.transform.rotation);
            var follow = moving.AddComponent<ArtPilotHorizontalVisuals>();
            var followData = new SerializedObject(follow); followData.FindProperty("body").objectReferenceValue = actor.transform; followData.ApplyModifiedPropertiesWithoutUndo();
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) throw new InvalidOperationException("Existing URP Sprite Unlit shader required.");
            var material = new Material(shader) { name = "PaletteC_v02_Prelit_Unlit" };
            AssetDatabase.CreateAsset(material, Data + "Prelit_Unlit.mat");
            foreach (var layer in manifest.layers)
            {
                string path = Art + layer.id + ".png";
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = layer.pixelsPerUnit; importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput; importer.alphaIsTransparency = false;
                importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
                importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
                importer.filterMode = FilterMode.Bilinear; importer.wrapMode = TextureWrapMode.Clamp;
                var textureSettings = new TextureImporterSettings(); importer.ReadTextureSettings(textureSettings);
                textureSettings.spriteMeshType = SpriteMeshType.FullRect; textureSettings.spritePivot = new Vector2(.5f, .5f);
                textureSettings.spriteAlignment = (int)SpriteAlignment.Center; importer.SetTextureSettings(textureSettings);
                importer.SaveAndReimport();
                var go = new GameObject(layer.id, typeof(SpriteRenderer));
                go.transform.SetParent(layer.followReferenceRootHorizontalDelta ? moving.transform : root.transform);
                go.transform.SetPositionAndRotation(new Vector3(0, 1.2f, .25f) + camera.transform.forward * layer.forwardDepthOffsetMeters, camera.transform.rotation);
                var renderer = go.GetComponent<SpriteRenderer>(); renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                renderer.sharedMaterial = material; renderer.sortingOrder = layer.sortingOrder;
                if (renderer.sprite == null || renderer.sprite.rect.width != 1920 || renderer.sprite.rect.height != 1080) throw new InvalidDataException("Full canvas required: " + layer.id);
            }
            var toggle = UnityEngine.Object.FindFirstObjectByType<ArtPilotReceiverToggle>();
            var toggleData = new SerializedObject(toggle);
            toggleData.FindProperty("receiverVisual").objectReferenceValue = GameObject.Find("PaletteC_v02_Integrated/receiver");
            toggleData.FindProperty("receiverShadow").objectReferenceValue = GameObject.Find("PaletteC_v02_Integrated/receiver_shadow");
            toggleData.ApplyModifiedPropertiesWithoutUndo();
            var marker = GameObject.Find("ArtPilot_PaletteC_PENDING_DESIGNER_LAYERS"); if (marker != null) UnityEngine.Object.DestroyImmediate(marker);
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("Pinned v02 art assigned to existing ArtPilot; prelit Unlit, camera-facing, no current-game changes.");
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create()) return string.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(value => value.ToString("x2")));
        }
    }
}
