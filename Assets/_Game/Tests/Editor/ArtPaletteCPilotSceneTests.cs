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
        public void IsolatedPilotReusesSavedGeometryAndCameraWithoutArtSubstitution()
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
            Assert.That(Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None), Is.Empty, "No old comparison screenshot substituted");
            Assert.That(Object.FindObjectsByType<Light>(FindObjectsSortMode.None), Is.Empty);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.path == pilot), Is.False);
            var toggle = new SerializedObject(Object.FindFirstObjectByType<ArtPilotReceiverToggle>());
            Assert.That(toggle.FindProperty("receiverVisual").objectReferenceValue, Is.Not.Null);
            Assert.That(toggle.FindProperty("receiverShadow").objectReferenceValue, Is.Null, "Actual designer shadow is pending, not fabricated");
        }
    }
}
