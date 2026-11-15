using System.Linq;
using DeadAir.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Tests
{
    public sealed class CurrentPlayableSceneTests
    {
        [Test]
        public void CurrentEntryRoutesToAssignedArtAndAudioWithExplicitOffArrival()
        {
            Assert.That(EditorBuildSettings.scenes[0].path, Is.EqualTo(CurrentPlayableInstaller.Entry));
            Assert.That(EditorBuildSettings.scenes.Take(2).All(scene => scene.enabled), Is.True);
            Assert.That(EditorBuildSettings.scenes[1].path, Is.EqualTo(CurrentPlayableInstaller.Motel));
            foreach (string path in new[] { CurrentPlayableInstaller.Entry, CurrentPlayableInstaller.Motel })
            {
                var scene = EditorSceneManager.OpenScene(path);
                var scripts = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MonoBehaviour>(true)).ToArray();
                Assert.That(scripts.All(script => script != null), Is.True, "No missing scripts");
                var camera = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Camera>(true)).Single();
                Assert.That(camera.orthographicSize, Is.EqualTo(2.25f));
                var audio = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<AudioSource>(true)).ToArray();
                Assert.That(audio.Length, Is.GreaterThanOrEqualTo(6));
                var props = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<SpriteRenderer>(true)).ToArray();
                Assert.That(props.Count(renderer => renderer.sprite != null && renderer.enabled), Is.GreaterThan(5));
                if (path == CurrentPlayableInstaller.Entry)
                {
                    var controller = scripts.OfType<PrologueController>().Single();
                    var definition = (PrologueDefinition)new SerializedObject(controller).FindProperty("definition").objectReferenceValue;
                    Assert.That(definition.TargetScenePath, Is.EqualTo(CurrentPlayableInstaller.Motel));
                    Assert.That(definition.PlayableCheckInArrival, Is.True);
                    Assert.That(scripts.OfType<PrologueAudio>().Single().Broadcast.clip, Is.Not.Null);
                }
                else
                {
                    var sound = scripts.OfType<FirstNightAudio>().Single();
                    var data = new SerializedObject(sound); var property = data.GetIterator();
                    while (property.NextVisible(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference) Assert.That(property.objectReferenceValue, Is.Not.Null, property.name);
                    Assert.That(scripts.OfType<RadioInput>().Single().enabled, Is.False);
                }
            }
        }
    }
}
