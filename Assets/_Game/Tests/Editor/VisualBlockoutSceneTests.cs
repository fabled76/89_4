using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Tests
{
    public sealed class VisualBlockoutSceneTests
    {
        [Test]
        public void SavedStudyKeepsMeasuredMeshesAndSeparateCamera()
        {
            const string path = "Assets/_Game/Scenes/Studies/VisualBlockout.unity";
            EditorSceneManager.OpenScene(path);
            var meshes = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
            Assert.That(meshes.Length, Is.EqualTo(49));
            foreach (var mesh in meshes)
            {
                Assert.That(mesh.sharedMesh, Is.Not.Null, mesh.name);
                Assert.That(AssetDatabase.Contains(mesh.sharedMesh), Is.True, mesh.name + " must survive reload");
                Assert.That(mesh.transform.lossyScale, Is.EqualTo(Vector3.one));
            }
            Size("Door_Leaf", new Vector3(.85f, 2.1f, .04f));
            Size("Desk_Top", new Vector3(1.35f, .04f, .65f));
            Size("Bed_Mattress", new Vector3(1.95f, .18f, .90f));
            Size("Receiver_Body", new Vector3(.18f, .11f, .05f));
            Size("Calibration_1m_Cube", Vector3.one);
            Assert.That(GameObject.Find("Door_Handle").transform.position.y, Is.EqualTo(1).Within(.0001f));
            Assert.That(GameObject.Find("Desk_Top").GetComponent<Renderer>().bounds.max.y, Is.EqualTo(.74f).Within(.0001f));
            Assert.That(GameObject.Find("Chair_Seat").GetComponent<Renderer>().bounds.max.y, Is.EqualTo(.45f).Within(.0001f));
            Assert.That(GameObject.Find("Bed_Mattress").GetComponent<Renderer>().bounds.max.y, Is.EqualTo(.50f).Within(.0001f));
            var figure = GameObject.Find("ReferencePerson");
            var parts = figure.GetComponentsInChildren<Renderer>();
            Assert.That(parts.Min(part => part.bounds.min.y), Is.EqualTo(0).Within(.0001f));
            Assert.That(parts.Max(part => part.bounds.max.y), Is.EqualTo(1.8f).Within(.0001f));
            Assert.That(GameObject.Find("Chair").GetComponentsInChildren<Renderer>().Min(part => part.bounds.min.z),
                Is.GreaterThan(parts.Max(part => part.bounds.max.z)), "Walking lane stays in front of chair");
            var camera = Camera.main;
            Assert.That(camera.orthographic, Is.True);
            Assert.That(camera.orthographicSize, Is.EqualTo(2.25f));
            Assert.That(camera.transform.eulerAngles.x, Is.EqualTo(18).Within(.001f));
            Assert.That(camera.WorldToViewportPoint(new Vector3(0, 0, .25f)).y,
                Is.LessThan(camera.WorldToViewportPoint(new Vector3(0, 0, 1.5f)).y));
            Assert.That(Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None), Is.Empty);
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.path == path), Is.False);
        }

        private static void Size(string name, Vector3 expected)
        {
            var actual = GameObject.Find(name).GetComponent<Renderer>().bounds.size;
            Assert.That(Vector3.Distance(actual, expected), Is.LessThan(.0001f), name);
        }
    }
}
