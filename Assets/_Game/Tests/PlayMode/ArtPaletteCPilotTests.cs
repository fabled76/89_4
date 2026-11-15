using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class ArtPaletteCPilotTests
    {
        [UnityTest]
        public IEnumerator ExistingEInteractionTogglesOnlyTheStudyReceiverAndAssignedShadow()
        {
            var oldSettings = InputSystem.settings;
            var settings = Object.Instantiate(oldSettings);
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = settings; var keyboard = InputSystem.AddDevice<Keyboard>();
            try
            {
#if UNITY_EDITOR
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/Studies/ArtPilot_PaletteC.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
                Assert.Ignore("Editor-only visual pilot.");
#endif
                var player = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
                var toggle = Object.FindFirstObjectByType<ArtPilotReceiverToggle>();
                yield return new WaitForSeconds(.15f);
                foreach (float x in new[] { -2.25f, -.4f, 1.3f, -.4f })
                    yield return Walk(player, keyboard, x);
                Assert.That(player.GetComponent<InteractionController>().FindNearest(), Is.SameAs(toggle));
                var receiver = GameObject.Find("MeasuredGeometry/Receiver");
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E)); yield return null; yield return null;
                Assert.That(receiver.activeSelf, Is.False);
                Assert.That(GameObject.Find("MeasuredGeometry/Desk").activeSelf, Is.True);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E)); yield return null; yield return null;
                Assert.That(receiver.activeSelf, Is.True);
                // Component contract only: a temporary empty object is not delivered shadow art.
                var shadow = new GameObject("TEST_ONLY_ShadowReference");
#if UNITY_EDITOR
                var data = new UnityEditor.SerializedObject(toggle);
                data.FindProperty("receiverShadow").objectReferenceValue = shadow; data.ApplyModifiedPropertiesWithoutUndo();
#endif
                toggle.SetVisible(false); Assert.That(shadow.activeSelf, Is.False);
                toggle.SetVisible(true); Assert.That(shadow.activeSelf, Is.True);
                Object.Destroy(shadow);
                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                InputSystem.RemoveDevice(keyboard); InputSystem.settings = oldSettings; Object.Destroy(settings);
            }
        }

        private static IEnumerator Walk(Rigidbody2D body, Keyboard keyboard, float x)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(x > body.position.x ? Key.D : Key.A));
            float deadline = Time.realtimeSinceStartup + 4;
            while (Mathf.Abs(body.position.x - x) > .12f && Time.realtimeSinceStartup < deadline) yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null; yield return null; yield return new WaitForFixedUpdate();
            Assert.That(body.position.x, Is.EqualTo(x).Within(.25f));
            Assert.That(body.position.y - .9f, Is.InRange(-.01f, .04f));
            Assert.That(body.transform.position.z, Is.EqualTo(.25f));
        }
    }
}
