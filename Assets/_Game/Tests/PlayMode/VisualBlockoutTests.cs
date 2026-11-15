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
    public sealed class VisualBlockoutTests
    {
        [UnityTest]
        public IEnumerator ExistingADControllerTraversesStudyAndWallsStopIt()
        {
            var originalSettings = InputSystem.settings;
            var settings = Object.Instantiate(originalSettings);
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = settings;
            var keyboard = InputSystem.AddDevice<Keyboard>();
            try
            {
#if UNITY_EDITOR
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/Studies/VisualBlockout.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
                Assert.Ignore("Separate Editor study, intentionally outside build scenes.");
#endif
                var body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
                yield return new WaitForSeconds(.2f);
                foreach (float x in new[] { -2.25f, -.4f, 1.3f })
                {
                    InputSystem.QueueStateEvent(keyboard, new KeyboardState(x > body.position.x ? Key.D : Key.A));
                    float deadline = Time.realtimeSinceStartup + 4;
                    while (Mathf.Abs(body.position.x - x) > .12f && Time.realtimeSinceStartup < deadline) yield return null;
                    InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                    yield return new WaitForFixedUpdate(); yield return null;
                    Assert.That(body.position.x, Is.EqualTo(x).Within(.25f));
                    Assert.That(body.position.y - .9f, Is.InRange(-.01f, .04f));
                    Assert.That(body.transform.position.z, Is.EqualTo(.25f));
                    if (x < 0)
                        Assert.That(((MonoBehaviour)body.GetComponent<InteractionController>().FindNearest()).name,
                            Is.EqualTo(x < -1 ? "Door_TransitionProxy" : "Receiver_ReviewProxy"));
                }
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
                yield return new WaitForSeconds(1.1f);
                Assert.That(body.position.x, Is.InRange(2.80f, 2.90f), "Right wall blocks the existing body");
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.A));
                yield return new WaitForSeconds(1.8f);
                Assert.That(body.position.x, Is.InRange(-2.90f, -2.80f), "Left wall blocks the existing body");
                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                InputSystem.RemoveDevice(keyboard); InputSystem.settings = originalSettings; Object.Destroy(settings);
            }
        }
    }
}
