using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class FirstNightV02Tests
    {
        private Keyboard keyboard;
        private InputSettings oldSettings, settings;
        private FirstNightController trial;
        private RadioController radio;
        private Rigidbody2D body;
        private bool hadPreference;
        private string preference;
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            hadPreference = PlayerPrefs.HasKey(PrototypeText.LocalePreferenceKey); preference = PlayerPrefs.GetString(PrototypeText.LocalePreferenceKey);
            oldSettings = InputSystem.settings; settings = Object.Instantiate(oldSettings);
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = settings; keyboard = InputSystem.AddDevice<Keyboard>();
            yield return Load();
        }
        private IEnumerator Load()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/MotelFirstNight_V02.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            Assert.Ignore("Editor asset integration.");
#endif
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            yield return LocalizationSettings.InitializationOperation; yield return null;
            trial = Object.FindFirstObjectByType<FirstNightController>(); radio = Object.FindFirstObjectByType<RadioController>();
            body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
        }
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            InputSystem.settings = oldSettings; Object.Destroy(settings);
            if (hadPreference) PlayerPrefs.SetString(PrototypeText.LocalePreferenceKey, preference); else PlayerPrefs.DeleteKey(PrototypeText.LocalePreferenceKey);
            PlayerPrefs.Save(); yield return null;
        }
        private IEnumerator Press(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
        }
        private IEnumerator Finish()
        {
            Assert.That(trial.Reader.IsOpen, Is.True);
            int count = trial.Reader.ActiveDocument.PageCount;
            for (int i = 0; i < count; i++) yield return Press(Key.D);
            Assert.That(trial.Reader.IsOpen, Is.False);
        }
        private IEnumerator Read(FirstNightAction action) { trial.Use(action); yield return Finish(); }
        private IEnumerator Room()
        {
            yield return Read(FirstNightAction.CheckIn); trial.Use(FirstNightAction.GuestDoor); yield return null;
        }
        private IEnumerator Meet()
        {
            trial.Use(FirstNightAction.CorridorDoor); yield return null;
            yield return Read(FirstNightAction.Man);
        }
        [UnityTest]
        public IEnumerator StandardRouteHasNoRadioPrerequisiteAndBothReturnsAreEmpty()
        {
            foreach (bool returnOn in new[] { true, false })
            {
                if (!returnOn) yield return Load();
                Assert.That(radio.IsOn || trial.HasReceiver, Is.False);
                Assert.That(radio.GetComponent<RadioInput>().enabled, Is.False);
                yield return Press(Key.LeftShift); Assert.That(radio.IsOn, Is.False);
                yield return Room(); Assert.That(trial.MovementHeard, Is.True);
                yield return Read(FirstNightAction.ReviewReceiver); Assert.That(trial.HasReceiver, Is.False);
                trial.Use(FirstNightAction.CorridorDoor); yield return null;
                Assert.That(trial.ManVisible && !trial.HasReceiver, Is.True);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D)); yield return new WaitForSeconds(1.6f);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
                Assert.That(body.position.x, Is.InRange(1.8f, 3)); Assert.That(body.position.y, Is.InRange(.88f, .98f));
                yield return Press(Key.E); yield return Finish();
                Assert.That(trial.Introduced && trial.ManVisible && !trial.ManDeparted, Is.True);
                trial.Use(FirstNightAction.ReturnToRoom); yield return null;
                Assert.That(trial.ManDeparted && !trial.HasReceiver && !radio.IsOn, Is.True);
                trial.Use(FirstNightAction.RetrieveReceiver); Assert.That(trial.HasReceiver, Is.True);
                yield return Press(Key.Escape); Assert.That(trial.HasReceiver, Is.True, "Canceling instructions must not undo pickup.");
                yield return Press(Key.LeftShift); Assert.That(radio.IsOn, Is.True);
                yield return Read(FirstNightAction.ReviewReceiver); Assert.That(trial.ProgramReviewed, Is.True);
                if (!returnOn) yield return Press(Key.LeftShift);
                trial.Use(FirstNightAction.CorridorDoor); yield return null; yield return null;
                Assert.That(trial.ReturnedEmpty && !trial.ManVisible, Is.True);
                Assert.That(radio.IsOn, Is.EqualTo(returnOn));
                yield return Finish();
                radio.Toggle(); yield return null; Assert.That(trial.ManVisible, Is.False);
                Assert.That(Object.FindFirstObjectByType<InvestigationStateTracker>(), Is.Null);
                Assert.That(Object.FindFirstObjectByType<EvidenceJournal>(), Is.Null);
            }
            yield return PrologueRenderCapture.Capture("first-night-v02-empty", 1280);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator EarlyPickupAndPrematureOnRemainRecoverableWithoutAnObservedFlag()
        {
            trial.Use(FirstNightAction.GuestDoor); yield return Finish(); Assert.That(trial.CheckedIn, Is.False);
            yield return Room(); yield return Read(FirstNightAction.RetrieveReceiver);
            Assert.That(GameObject.Find("ReceiverReview").GetComponent<SpriteRenderer>().enabled, Is.False);
            yield return Press(Key.LeftShift); yield return Read(FirstNightAction.ReviewReceiver);
            Assert.That(trial.ProgramReviewed, Is.False, "Early listening is available but is not a post-claim check.");
            trial.Use(FirstNightAction.CorridorDoor); yield return null; Assert.That(trial.ManVisible, Is.False);
            yield return Press(Key.LeftShift); trial.Use(FirstNightAction.Man); yield return Press(Key.D);
            yield return Press(Key.LeftShift); Assert.That(trial.Reader.IsOpen || trial.Introduced || trial.ManDeparted, Is.False);
            Assert.That(body.GetComponent<PlayerController2D>().enabled, Is.True);
            yield return Press(Key.LeftShift); trial.Use(FirstNightAction.Man); yield return Press(Key.Escape);
            trial.Use(FirstNightAction.ReturnToRoom); yield return null;
            Assert.That(trial.ManDeparted, Is.False, "An incomplete introduction does not remove a required conversation.");
            yield return Meet(); trial.Use(FirstNightAction.ReturnToRoom); yield return null;
            Assert.That(trial.ManDeparted && trial.HasReceiver, Is.True);
            yield return Press(Key.LeftShift); yield return Read(FirstNightAction.ReviewReceiver);
            Assert.That(trial.ProgramReviewed, Is.True);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator EarlyEmptyReturnExhaustionAndCanceledProgramDoNotDeadlock()
        {
            yield return Room(); yield return Meet(); trial.Use(FirstNightAction.ReturnToRoom); yield return null;
            trial.Use(FirstNightAction.CorridorDoor); yield return null; yield return null;
            Assert.That(trial.ReturnedEmpty && !trial.ProgramReviewed && !trial.HasReceiver, Is.True);
            yield return Press(Key.Escape);
            trial.Use(FirstNightAction.ReturnToRoom); yield return null;
            yield return Read(FirstNightAction.RetrieveReceiver);
            var battery = radio.GetComponent<BatteryComponent>(); radio.SetPower(true); battery.Consume(battery.Capacity);
            yield return Press(Key.LeftShift); Assert.That(radio.IsOn, Is.False);
            trial.Use(FirstNightAction.TestBattery); Assert.That(radio.IsOn, Is.False);
            yield return Press(Key.LeftShift); trial.Use(FirstNightAction.ReviewReceiver); yield return Press(Key.D); yield return Press(Key.Escape);
            Assert.That(trial.ProgramReviewed, Is.False);
            yield return Read(FirstNightAction.ReviewReceiver);
            float charge = battery.Charge; radio.Tick(100000); Assert.That(battery.Charge, Is.EqualTo(charge));
            trial.Use(FirstNightAction.CorridorDoor); yield return null; yield return null;
            Assert.That(trial.Reader.IsOpen, Is.False, "The automatic empty notice is shown once.");
            Assert.That(radio.IsOn && trial.ProgramReviewed && !trial.ManVisible, Is.True);
            yield return Read(FirstNightAction.LookAround);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator AllWorkingDocumentsFitENRUWithoutAudio()
        {
            Assert.That(Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None), Is.Empty);
            foreach (string code in new[] { "en", "ru" })
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
                yield return LocalizationSettings.InitializationOperation; yield return null;
#if UNITY_EDITOR
                foreach (string name in new[] { "CheckIn", "Inspection", "Retrieval", "RetrievalAfterClaim", "EarlyProgram", "Program", "Introduction", "EmptyCorridor", "NeedCheckIn", "NeedPower" })
                {
                    var document = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>("Assets/_Game/Data/MotelFirstNightV02/" + name + ".asset");
                    Assert.That(trial.Reader.Show(document), Is.True);
                    for (int page = 0; page < document.PageCount; page++)
                    {
                        foreach (int width in new[] { 1280, 960 })
                            yield return PrologueRenderCapture.Capture("first-night-v02-" + code + "-" + name + "-" + page, width);
                        Assert.That(document.GetPage(page), Does.Not.Contain("No translation found"));
                        yield return Press(Key.D);
                    }
                }
#endif
            }
            LogAssert.NoUnexpectedReceived();
        }
    }
}
