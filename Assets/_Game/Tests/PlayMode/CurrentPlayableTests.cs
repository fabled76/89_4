using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class CurrentPlayableTests
    {
        private Keyboard keyboard;
        private InputSettings oldSettings, settings;
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
            InputSystem.settings = settings; keyboard = InputSystem.AddDevice<Keyboard>(); yield return null;
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
        private IEnumerator Walk(float x)
        {
            float deadline = Time.realtimeSinceStartup + 7;
            Key key = x > body.position.x ? Key.D : Key.A;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
            while (Mathf.Abs(body.position.x - x) > .12f && Time.realtimeSinceStartup < deadline) yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return new WaitForFixedUpdate(); yield return null;
            Assert.That(body.position.x, Is.EqualTo(x).Within(.25f), "Walking must reach " + x);
            Assert.That(body.position.y - .9f, Is.InRange(.48f, .54f), "Feet must remain in the supplied floor band.");
        }
        private IEnumerator Read()
        {
            yield return Press(Key.E);
            var reader = Object.FindFirstObjectByType<PrologueReadingPanel>(); Assert.That(reader.IsOpen, Is.True);
            int pages = reader.ActiveDocument != null ? reader.ActiveDocument.PageCount : 1;
            for (int i = 0; i < pages; i++) yield return Press(Key.D);
            Assert.That(reader.IsOpen, Is.False);
        }
        private IEnumerator Shot(string name, string code)
        {
            foreach (int width in new[] { 1280, 960 }) yield return CurrentRouteCapture.Save(name + "-" + code, width);
        }
        [UnityTest]
        public IEnumerator RuntimeAudioHandlesLocalePowerMissingClipsAndDisableCleanly()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/CurrentMotel.unity", new LoadSceneParameters(LoadSceneMode.Single));
#endif
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            yield return LocalizationSettings.InitializationOperation; yield return null;
            var trial = Object.FindFirstObjectByType<FirstNightController>(); var audio = Object.FindFirstObjectByType<FirstNightAudio>();
            trial.Use(FirstNightAction.CheckIn);
            for (int i = 0; i < 5; i++) yield return Press(Key.D);
            trial.Use(FirstNightAction.GuestDoor); yield return null;
            trial.Use(FirstNightAction.RetrieveReceiver); Assert.That(GameObject.Find("Audio_handling").GetComponent<AudioSource>().isPlaying, Is.True);
            trial.Reader.Dismiss(false);
            yield return Press(Key.LeftShift); trial.Use(FirstNightAction.ReviewReceiver);
            Assert.That(audio.Voice.isPlaying && audio.Texture.isPlaying && audio.Ambience.isPlaying, Is.True);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("ru");
            yield return LocalizationSettings.InitializationOperation; yield return null;
            Assert.That(audio.Voice.isPlaying, Is.False);
            Assert.That(audio.Texture.isPlaying, Is.True);
            yield return Press(Key.LeftShift); Assert.That(audio.Texture.isPlaying || audio.Voice.isPlaying, Is.False);
            Assert.That(audio.Ambience.isPlaying, Is.True);
            trial.Reader.Dismiss(false);
            audio.enabled = false;
            foreach (var source in audio.GetComponentsInChildren<AudioSource>()) Assert.That(source.isPlaying, Is.False);
            audio.enabled = true; yield return null; Assert.That(audio.Ambience.isPlaying, Is.True);
            audio.Voice.clip = null; audio.Texture.clip = null;
            yield return Press(Key.LeftShift); trial.Use(FirstNightAction.ReviewReceiver);
            Assert.That(trial.Reader.IsOpen, Is.True, "Missing clips never block readable program review.");
            yield return Press(Key.Escape);
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator FullCurrentRouteFromOneEntryWithPaintedFloorAndRuntimeAudio()
        {
            foreach (string code in new[] { "en", "ru" })
            {
#if UNITY_EDITOR
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/CurrentPrologue.unity", new LoadSceneParameters(LoadSceneMode.Single));
#endif
                yield return LocalizationSettings.InitializationOperation;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
                yield return LocalizationSettings.InitializationOperation; yield return null; yield return new WaitForFixedUpdate();
                body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
                Assert.That(Object.FindFirstObjectByType<PrologueAudio>().Broadcast.isPlaying, Is.True);
                yield return Walk(2.12f); yield return Shot("01-home-phone", code); yield return Read();
                yield return Walk(3.9f); yield return Shot("02-home-desk", code); yield return Read();
                var prologue = Object.FindFirstObjectByType<PrologueController>(); Assert.That(prologue.ReadyToPack, Is.True);
                yield return Walk(5.14f); yield return Read(); Assert.That(prologue.BagPrepared, Is.True);
                Assert.That(GameObject.Find("DeskRadio").GetComponent<SpriteRenderer>().enabled, Is.False);
                yield return Walk(6.32f); yield return Shot("03-home-door", code);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E)); yield return null; yield return null;
                float deadline = Time.realtimeSinceStartup + 15;
                while (SceneManager.GetActiveScene().path != "Assets/_Game/Scenes/CurrentMotel.unity" && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo("Assets/_Game/Scenes/CurrentMotel.unity"));
                yield return null; yield return null;
                var trial = Object.FindFirstObjectByType<FirstNightController>(); var radio = Object.FindFirstObjectByType<RadioController>();
                var audio = Object.FindFirstObjectByType<FirstNightAudio>();
                body = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
                Assert.That(trial.HasReceiver || radio.IsOn, Is.False);
                Assert.That(body.GetComponent<PlayerController2D>().enabled, Is.False, "Held E cannot leak through arrival.");
                InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null; yield return null;
                Assert.That(Object.FindFirstObjectByType<PrologueJourney>(), Is.Null, "Current route has no legacy bridge acknowledgement.");
                Assert.That(body.GetComponent<PlayerController2D>().enabled, Is.True);
                Assert.That(audio.Ambience.isPlaying && !audio.Texture.isPlaying && !audio.Voice.isPlaying, Is.True);
                yield return Shot("04-reception", code); yield return Walk(6.2f); yield return Read();
                Assert.That(trial.CheckedIn, Is.True);
                yield return Walk(1.8f); yield return Press(Key.E); yield return null;
                Assert.That(trial.Area, Is.EqualTo(FirstNightArea.GuestRoom));
                yield return Walk(3); yield return Shot("05-guest-before-pickup", code);
                Assert.That(trial.HasReceiver || radio.IsOn, Is.False);
                yield return Walk(1.5f); yield return Press(Key.E); yield return null;
                Assert.That(trial.Area, Is.EqualTo(FirstNightArea.Corridor));
                yield return Walk(4.55f); yield return Shot("06-corridor-meeting", code); yield return Read();
                Assert.That(trial.Introduced && trial.ManVisible && !trial.ManDeparted, Is.True);
                yield return Walk(1.92f); yield return Press(Key.E); yield return null;
                Assert.That(trial.ManDeparted, Is.True);
                yield return Walk(3.64f); yield return Read();
                Assert.That(trial.HasReceiver, Is.True);
                yield return Press(Key.LeftShift); Assert.That(audio.Texture.isPlaying, Is.True);
                yield return Press(Key.E);
                Assert.That(audio.Voice.isPlaying, Is.EqualTo(code == "en"));
                yield return Press(Key.Escape); Assert.That(audio.Voice.isPlaying, Is.False);
                yield return Read(); Assert.That(trial.ProgramReviewed, Is.True);
                yield return Walk(3); yield return Shot("07-guest-carried", code);
                yield return Walk(1.5f); yield return Press(Key.E); yield return null; yield return null;
                Assert.That(trial.ReturnedEmpty && !trial.ManVisible, Is.True);
                yield return Press(Key.Escape); yield return Walk(4.55f); yield return Shot("08-corridor-empty", code);
                yield return Press(Key.LeftShift); Assert.That(audio.Texture.isPlaying || audio.Voice.isPlaying, Is.False);
                Assert.That(audio.Ambience.isPlaying && !trial.ManVisible, Is.True);
                Assert.That(Object.FindFirstObjectByType<InvestigationStateTracker>(), Is.Null);
                LogAssert.NoUnexpectedReceived();
            }
        }
    }
}
