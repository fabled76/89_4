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
        public IEnumerator IntegratedV02TraversesAndRemovesReceiverWithShadowAndCapturesUnityFrames()
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
                var receiver = GameObject.Find("PaletteC_v02_Integrated/receiver");
                var shadow = GameObject.Find("PaletteC_v02_Integrated/receiver_shadow");
                var following = Object.FindFirstObjectByType<ArtPilotHorizontalVisuals>().transform;
                yield return ArtPaletteCPilotCapture.Save("reference-present", 1920);
                toggle.SetVisible(false);
                yield return ArtPaletteCPilotCapture.Save("reference-removed", 1920);
                toggle.SetVisible(true);
                int position = 0;
                // Stand beside the receiver rather than covering it with the foreground actor.
                foreach (float x in new[] { -2.25f, .1f, 1.3f, .1f })
                {
                    yield return Walk(player, keyboard, x);
                    Assert.That(following.position.x, Is.EqualTo(player.position.x - .9f).Within(.001f));
                    Assert.That(following.position.y, Is.EqualTo(1.2f));
                    Assert.That(following.position.z, Is.EqualTo(.25f));
                    if (position++ < 3)
                        foreach (int width in new[] { 1280, 960 }) yield return ArtPaletteCPilotCapture.Save("position-" + position, width);
                }
                Assert.That(player.GetComponent<InteractionController>().FindNearest(), Is.SameAs(toggle));
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E)); yield return null; yield return null;
                Assert.That(receiver.activeSelf || shadow.activeSelf, Is.False);
                Assert.That(GameObject.Find("MeasuredGeometry/Desk").activeSelf, Is.True);
                foreach (int width in new[] { 1280, 960 }) yield return ArtPaletteCPilotCapture.Save("desk-receiver-removed", width);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E)); yield return null; yield return null;
                Assert.That(receiver.activeSelf && shadow.activeSelf, Is.True);
                foreach (int width in new[] { 1280, 960 }) yield return ArtPaletteCPilotCapture.Save("desk-receiver-restored", width);
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
            yield return new WaitForSeconds(.08f); yield return null;
            Assert.That(body.position.x, Is.EqualTo(x).Within(.25f));
            Assert.That(body.position.y - .9f, Is.InRange(-.01f, .04f));
            Assert.That(body.transform.position.z, Is.EqualTo(.25f));
        }
    }
}
