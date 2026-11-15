using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Tests
{
    public sealed class ArtPaletteCPilotSceneTests
    {
        [Test]
        public void IntegratedV03KeepsMeasuredGeometryAndCameraAndUsesFivePrelitLayers()
        {
            const string pilot = "Assets/_Game/Scenes/Studies/ArtPilot_PaletteC.unity";
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Studies/VisualBlockout.unity");
            var source = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None)
                .ToDictionary(mesh => mesh.name, mesh => (mesh.sharedMesh, mesh.transform.position, mesh.transform.lossyScale));
            Vector3 cameraPosition = Camera.main.transform.position;
            Quaternion rotation = Camera.main.transform.rotation;
            EditorSceneManager.OpenScene(pilot);
            var meshes = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
            Assert.That(meshes.Length, Is.EqualTo(source.Count));
            foreach (var mesh in meshes)
            {
                var original = source[mesh.name];
                Assert.That(mesh.sharedMesh, Is.SameAs(original.sharedMesh));
                Assert.That(mesh.transform.position, Is.EqualTo(original.position));
                Assert.That(mesh.transform.lossyScale, Is.EqualTo(original.lossyScale));
            }
            Assert.That(Camera.main.transform.position, Is.EqualTo(cameraPosition));
            Assert.That(Camera.main.transform.rotation, Is.EqualTo(rotation));
            Assert.That(Camera.main.orthographicSize, Is.EqualTo(2.25f));
            Assert.That(meshes.All(mesh => !mesh.GetComponent<MeshRenderer>().enabled), Is.True, "Superseded meshes must not leak through art");
            var sprites = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            Assert.That(sprites.Length, Is.EqualTo(5));
            foreach (var sprite in sprites)
            {
                string path = AssetDatabase.GetAssetPath(sprite.sprite);
                Assert.That(path.StartsWith("Assets/_Game/Art/Studies/PaletteC_v03/"), Is.True);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                Assert.That(importer.sRGBTexture && !importer.mipmapEnabled, Is.True);
                Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
                Assert.That(sprite.sprite.rect.size, Is.EqualTo(new Vector2(1920, 1080)));
                Assert.That(sprite.sprite.pixelsPerUnit, Is.EqualTo(240));
                Assert.That(Quaternion.Angle(sprite.transform.rotation, rotation), Is.LessThan(.001f));
                Assert.That(Vector3.Distance(sprite.transform.lossyScale, Vector3.one), Is.LessThan(.00001f));
                Assert.That(sprite.sharedMaterial.shader.name, Is.EqualTo("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
            }
            Assert.That(Object.FindObjectsByType<Light>(FindObjectsSortMode.None), Is.Empty);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.path == pilot), Is.False);
            var toggle = new SerializedObject(Object.FindFirstObjectByType<ArtPilotReceiverToggle>());
            Assert.That(toggle.FindProperty("receiverVisual").objectReferenceValue, Is.Not.Null);
            Assert.That(toggle.FindProperty("receiverShadow").objectReferenceValue, Is.Not.Null);
        }
    }
}
