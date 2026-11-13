using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.Localization.Settings;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class MotelCorridorTests
    {
        private Keyboard keyboard;
        private bool hadLocalePreference;
        private string localePreference;
        private InputSettings previousSettings;
        private InputSettings testSettings;

        [UnitySetUp]
        public IEnumerator LoadCorridor()
        {
            hadLocalePreference = PlayerPrefs.HasKey(PrototypeText.LocalePreferenceKey);
            localePreference = PlayerPrefs.GetString(PrototypeText.LocalePreferenceKey);
            previousSettings = InputSystem.settings;
            testSettings = Object.Instantiate(previousSettings);
            testSettings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            testSettings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = testSettings;
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                "Assets/_Game/Scenes/MotelCorridor.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            Assert.Ignore("These integration tests load the prototype directly from its editor asset path.");
#endif
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            yield return null;
            keyboard = InputSystem.AddDevice<Keyboard>();
            yield return null;
            yield return new WaitForFixedUpdate();
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            InputSystem.settings = previousSettings;
            Object.Destroy(testSettings);
            if (hadLocalePreference) PlayerPrefs.SetString(PrototypeText.LocalePreferenceKey, localePreference);
            else PlayerPrefs.DeleteKey(PrototypeText.LocalePreferenceKey);
            PlayerPrefs.Save();
            yield return null;
        }

        private IEnumerator Press(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
        }

        [UnityTest]
        public IEnumerator KeyboardRadioWorldHudAndBatteryRecovery()
        {
            var radio = Object.FindFirstObjectByType<RadioController>();
            var battery = Object.FindFirstObjectByType<BatteryComponent>();
            var drift = GameObject.Find("Environment").transform.Find("DRIFT_Visuals").gameObject;
            var stable = GameObject.Find("Environment").transform.Find("STABLE_Visuals").gameObject;
            var text = GameObject.Find("RadioAndControls").GetComponent<TMP_Text>();
            Assert.That(radio.IsOn, Is.True);
            Assert.That(stable.activeSelf, Is.True);
            Assert.That(drift.activeSelf, Is.False);
            Assert.That(text.text, Does.Contain("ON / STABLE"));
            float before = battery.Charge;
            yield return new WaitForSeconds(.1f);
            Assert.That(battery.Charge, Is.LessThan(before));

            yield return Press(Key.LeftShift);
            Assert.That(radio.IsOn, Is.False);
            Assert.That(drift.activeSelf, Is.True);
            Assert.That(stable.activeSelf, Is.False);
            Assert.That(drift.transform.Find("Silhouette_VisualOnly_NoAI").gameObject.activeInHierarchy, Is.True);
            before = battery.Charge;
            yield return new WaitForSeconds(.1f);
            Assert.That(battery.Charge, Is.EqualTo(before));

            yield return Press(Key.LeftShift);
            battery.Consume(battery.Capacity * 2f);
            Assert.That(radio.IsOn, Is.False);
            Assert.That(drift.activeSelf, Is.True);
            Assert.That(text.text, Does.Contain("OFF / DRIFT"));
            yield return Press(Key.LeftShift);
            Assert.That(radio.IsOn, Is.False);

            var player = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
            player.position = new Vector2(-8, 1);
            player.linearVelocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            yield return null;
            LogAssert.Expect(LogType.Log, "Test battery replaced. Use Left Shift to power the radio if it is off.");
            yield return Press(Key.E);
            Assert.That(battery.Charge, Is.EqualTo(battery.Capacity));
            Assert.That(radio.IsOn, Is.False);
            Assert.That(text.text, Does.Contain("BATTERY 100"));
            yield return Press(Key.LeftShift);
            Assert.That(radio.IsOn, Is.True);
            Assert.That(drift.activeSelf, Is.False);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator MovementStopsAtBothWallsAndCameraStaysBounded()
        {
            var body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
            body.position = new Vector2(13, 1);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return new WaitForSeconds(1f);
            Assert.That(body.position.x, Is.GreaterThan(13.5f));
            Assert.That(body.position.x, Is.LessThan(14.7f));
            Assert.That(body.position.y, Is.GreaterThan(.7f));
            yield return new WaitForSeconds(.3f);
            Assert.That(body.position.x, Is.LessThan(14.7f));
            var view = Camera.main;
            float extent = view.orthographicSize * view.aspect;
            if (extent < 15f)
                Assert.That(view.transform.position.x + extent, Is.LessThanOrEqualTo(15.01f));

            body.position = new Vector2(-13, 1);
            body.linearVelocity = Vector2.zero;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.LeftArrow));
            yield return new WaitForSeconds(1f);
            Assert.That(body.position.x, Is.LessThan(-13.5f));
            Assert.That(body.position.x, Is.GreaterThan(-14.7f));
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return new WaitForSeconds(.1f);
            Assert.That(Mathf.Abs(body.linearVelocity.x), Is.LessThan(.01f));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ESelectsNearestEnabledCandidateAndRejectsOutOfRange()
        {
            var body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
            body.position = new Vector2(1, 1);
            body.linearVelocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            yield return null;
            var interactor = body.GetComponent<InteractionController>();
            var marker = GameObject.Find("TestInspectionMarker").GetComponent<InspectInteractable>();
            var duplicate = Object.Instantiate(marker.gameObject);
            duplicate.transform.position = new Vector2(2, .7f);
            var farther = duplicate.GetComponent<InspectInteractable>();
            Physics2D.SyncTransforms();
            Assert.That(interactor.FindNearest(), Is.SameAs(marker));
            LogAssert.Expect(LogType.Log, "Prototype interaction works. No story fact established.");
            yield return Press(Key.E);
            marker.enabled = false;
            Assert.That(interactor.FindNearest(), Is.SameAs(farther));
            body.position = new Vector2(5, 1);
            yield return new WaitForFixedUpdate();
            yield return null;
            Physics2D.SyncTransforms();
            Assert.That(interactor.FindNearest(), Is.Null);
            yield return Press(Key.E);
            LogAssert.NoUnexpectedReceived();
        }
    }
}
