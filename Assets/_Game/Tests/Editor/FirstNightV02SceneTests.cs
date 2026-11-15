using System.Linq;
using DeadAir.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeadAir.Tests
{
    public sealed class FirstNightV02SceneTests
    {
        [Test]
        public void V02HasNoHistoricalControllerAndAuthorsUncarriedOffWithoutArrivalAudio()
        {
            var scene = SceneManager.GetSceneByPath(FirstNightV02Builder.ScenePath);
            bool loaded = scene.IsValid() && scene.isLoaded;
            if (!loaded) scene = EditorSceneManager.OpenScene(FirstNightV02Builder.ScenePath, OpenSceneMode.Additive);
            try
            {
                var scripts = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MonoBehaviour>(true)).ToArray();
                foreach (var script in scripts)
                {
                    Assert.That(script, Is.Not.Null);
                    Assert.That(new[] { "MotelArrivalController", "MotelArrivalInteractable", "UnacceptedEncounter", "InvestigationStateTracker", "EvidenceJournal", "PrologueJourney" }, Does.Not.Contain(script.GetType().Name));
                    if (script.GetType().Namespace != "DeadAir") continue;
                    var property = new SerializedObject(script).GetIterator();
                    while (property.NextVisible(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference && property.name != "definition")
                            Assert.That(property.objectReferenceValue, Is.Not.Null, script.name + "." + property.propertyPath);
                }
                var radio = scripts.OfType<RadioController>().Single();
                Assert.That(new SerializedObject(radio).FindProperty("startPoweredOn").boolValue, Is.False);
                Assert.That(radio.GetComponent<RadioInput>().enabled, Is.False);
                Assert.That(scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<AudioSource>(true)), Is.Empty);
                Assert.That(EditorBuildSettings.scenes[0].path, Is.EqualTo(CurrentPlayableInstaller.Entry));
            }
            finally { if (!loaded) EditorSceneManager.CloseScene(scene, true); }
        }
    }
}
