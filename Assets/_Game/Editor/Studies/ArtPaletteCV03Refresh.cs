using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Editor.Studies
{
    public static class ArtPaletteCV03Refresh
    {
        public const string Data = "Assets/_Game/Data/Studies/PaletteC_v03/";
        public const string Art = "Assets/_Game/Art/Studies/PaletteC_v03/";
        [Serializable] private sealed class Manifest { public Layer[] layers; }
        [Serializable] private sealed class Layer { public string id, sha256; public int sortingOrder; }

        public static void Install()
        {
            if (Hash(Data + "manifest.json") != "fc2de82c4f78d65f51508ff275e5b137d8b6d634cde0cf71453756587996fe3d")
                throw new InvalidDataException("Pinned v03 manifest changed.");
            var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(Data + "manifest.json"));
            if (manifest.layers.Length != 5) throw new InvalidDataException("Exactly five compatible v03 layers required.");
            foreach (var layer in manifest.layers)
                if (Hash(Art + layer.id + ".png") != layer.sha256) throw new InvalidDataException("v03 source mismatch: " + layer.id);
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(ArtPaletteCPilotSetup.ScenePath);
            if (GameObject.Find("PaletteC_v03_Integrated") != null) return;
            var root = GameObject.Find("PaletteC_v02_Integrated");
            if (root == null) throw new InvalidOperationException("Existing integrated v02 pilot required; no scene rebuild is allowed.");
            var sprites = root.GetComponentsInChildren<SpriteRenderer>(true);
            if (sprites.Length != 5) throw new InvalidOperationException("Review manual pilot edits before applying a five-layer refresh.");
            var material = new Material(sprites[0].sharedMaterial) { name = "PaletteC_v03_Prelit_Unlit" };
            AssetDatabase.CreateAsset(material, Data + "Prelit_Unlit.mat");
            foreach (var layer in manifest.layers)
            {
                string path = Art + layer.id + ".png";
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 240; importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput; importer.alphaIsTransparency = false;
                importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
                importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
                importer.filterMode = FilterMode.Bilinear; importer.wrapMode = TextureWrapMode.Clamp;
                var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect; settings.spritePivot = new Vector2(.5f, .5f);
                settings.spriteAlignment = (int)SpriteAlignment.Center; importer.SetTextureSettings(settings); importer.SaveAndReimport();
                var renderer = sprites.Single(sprite => sprite.name == layer.id);
                renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                renderer.sharedMaterial = material; renderer.sortingOrder = layer.sortingOrder;
            }
            // Existing camera-facing transforms, following root, body, proxy and toggle references stay intact.
            root.name = "PaletteC_v03_Integrated";
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("Existing ArtPilot refreshed to pinned v03; v02 assets and gameplay preserved.");
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create()) return string.Concat(sha.ComputeHash(File.ReadAllBytes(path)).Select(value => value.ToString("x2")));
        }
    }
}
