using DeadAir.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeadAir.Tests
{
    public sealed class CorridorSceneTests
    {
        [Test]
        public void SceneHasNoMissingScriptsOrUnassignedObjectReferences()
        {
            if (SceneManager.loadedSceneCount == 0)
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var scene = SceneManager.GetSceneByPath(MotelPrototypeBuilder.ScenePath);
            bool alreadyLoaded = scene.IsValid() && scene.isLoaded;
            if (!alreadyLoaded)
                scene = EditorSceneManager.OpenScene(MotelPrototypeBuilder.ScenePath, OpenSceneMode.Additive);
            try
            {
                int components = 0;
                foreach (var root in scene.GetRootGameObjects())
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                {
                    Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject), Is.Zero, transform.name);
                    foreach (var behaviour in transform.GetComponents<MonoBehaviour>())
                    {
                        if (!behaviour.GetType().Namespace.StartsWith("DeadAir")) continue;
                        components++;
                        var property = new SerializedObject(behaviour).GetIterator();
                        while (property.NextVisible(true))
                        {
                            if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                            Assert.That(property.objectReferenceValue, Is.Not.Null, behaviour.name + "." + property.propertyPath);
                        }
                    }
                }
                Assert.That(components, Is.GreaterThanOrEqualTo(9));
                var environment = System.Array.Find(scene.GetRootGameObjects(), root => root.name == "Environment").transform;
                Assert.That(environment.Find("STABLE_Visuals").GetComponentsInChildren<Collider2D>(true), Is.Empty);
                Assert.That(environment.Find("DRIFT_Visuals").GetComponentsInChildren<Collider2D>(true), Is.Empty);
                Assert.That(environment.Find("SharedCollision").GetComponentsInChildren<Collider2D>().Length, Is.EqualTo(4));
            }
            finally
            {
                if (!alreadyLoaded) EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
