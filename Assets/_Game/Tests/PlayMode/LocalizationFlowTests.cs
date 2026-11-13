using System.Collections;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Localization.Settings;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class LocalizationFlowTests
    {
        private Keyboard keyboard;
        private InputSettings originalSettings, testSettings;
        private bool hadPreference;
        private string preference;
        private InvestigationUI ui;
        private EvidenceJournal journal;
        private LanguageSettingsPanel languages;
        private Rigidbody2D player;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            hadPreference = PlayerPrefs.HasKey(PrototypeText.LocalePreferenceKey);
            preference = PlayerPrefs.GetString(PrototypeText.LocalePreferenceKey);
            originalSettings = InputSystem.settings;
            testSettings = Object.Instantiate(originalSettings);
            testSettings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            testSettings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = testSettings;
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/MotelCorridor.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            Assert.Ignore("Editor asset integration tests.");
#endif
            yield return LocalizationSettings.InitializationOperation;
            keyboard = InputSystem.AddDevice<Keyboard>();
            languages = Object.FindFirstObjectByType<LanguageSettingsPanel>();
            ui = Object.FindFirstObjectByType<InvestigationUI>();
            journal = Object.FindFirstObjectByType<EvidenceJournal>();
            player = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
            yield return Switch("en");
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            InputSystem.settings = originalSettings;
            Object.Destroy(testSettings);
            if (hadPreference) PlayerPrefs.SetString(PrototypeText.LocalePreferenceKey, preference);
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
        private IEnumerator Switch(string code)
        {
            languages.SelectLocale(code);
            yield return LocalizationSettings.InitializationOperation;
            yield return LocalizationSettings.StringDatabase.GetTableAsync(PrototypeText.TableName, LocalizationSettings.AvailableLocales.GetLocale(code));
            yield return null;
        }
        private IEnumerator MoveTo(float x)
        {
            player.position = new Vector2(x, 1);
            player.linearVelocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return null;
        }
        private TMP_Text Body => GameObject.Find("PrototypeHUD").GetComponentsInChildren<TMP_Text>(true).Single(text => text.name == "Body");

        [UnityTest]
        public IEnumerator SettingsSwitchUpdatesOpenPageAndRussianEncounterKeepsStableIds()
        {
            yield return Press(Key.F1);
            yield return Press(Key.Digit2);
            yield return LocalizationSettings.InitializationOperation;
            yield return null;
            Assert.That(LocalizationSettings.SelectedLocale.Identifier.Code, Is.EqualTo("ru"));
            Assert.That(GameObject.Find("RadioAndControls").GetComponent<TMP_Text>().text, Does.Contain("РАДИО"));
            Assert.That(PlayerPrefs.GetString(PrototypeText.LocalePreferenceKey), Is.EqualTo("ru"));
            yield return Press(Key.F1);
            yield return MoveTo(-5);
            Assert.That(player.GetComponent<InteractionController>().CurrentPrompt, Does.Contain("гостевую книгу"));
            yield return Press(Key.E);
            yield return Press(Key.Space);
            var ledger = ui.ActiveEvidence;
            Assert.That(ledger.Id, Is.EqualTo("motel.guest-ledger"));
            yield return Switch("en");
            Assert.That(Body.text, Is.EqualTo(ledger.GetPage(1)), "Language switch must retain current reading page.");
            yield return Switch("ru");
            Assert.That(Body.text, Is.EqualTo(ledger.GetPage(1)));
            for (int i = 1; i < ledger.PageCount; i++) yield return Press(Key.Space);
            Assert.That(journal.Entries.Count, Is.EqualTo(1));
            yield return MoveTo(7);
            yield return Press(Key.LeftShift);
            yield return new WaitForFixedUpdate();
            Assert.That(ui.IsOpen, Is.True);
            var testimony = ui.ActiveEvidence;
            Assert.That(testimony.Id, Is.EqualTo("witness.station-lead"));
            for (int i = 0; i < testimony.PageCount; i++) yield return Press(Key.Space);
            Assert.That(Object.FindFirstObjectByType<InvestigationStateTracker>().TestimonyReceived, Is.True);
            string[] ids = journal.Entries.Select(e => e.Id).ToArray();
            yield return Press(Key.Tab);
            yield return Press(Key.RightArrow);
            string russianPage = Body.text;
            yield return Switch("en");
            Assert.That(Body.text, Is.EqualTo(ledger.GetPage(1)));
            yield return Switch("ru");
            Assert.That(Body.text, Is.EqualTo(russianPage));
            CollectionAssert.AreEqual(ids, journal.Entries.Select(e => e.Id));
            yield return Press(Key.Tab);
            Assert.That(ui.IsOpen, Is.False);
            Assert.That(player.GetComponent<PlayerController2D>().enabled, Is.True);
            var selector = new PlayerPrefLocaleSelector { PlayerPreferenceKey = PrototypeText.LocalePreferenceKey };
            Assert.That(selector.GetStartupLocale(LocalizationSettings.AvailableLocales).Identifier.Code, Is.EqualTo("ru"));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator AllEnglishAndRussianEvidencePagesRenderWithoutOverflow()
        {
            foreach (string code in new[] { "en", "ru" })
            {
                yield return Switch(code);
                foreach (var evidence in Resources.FindObjectsOfTypeAll<EvidenceDefinition>())
                {
                    Assert.That(ui.ShowEvidence(evidence, null), Is.True);
                    for (int i = 0; i < evidence.PageCount; i++)
                    {
                        yield return null;
                        Canvas.ForceUpdateCanvases();
                        Body.ForceMeshUpdate();
                        Assert.That(Body.isTextOverflowing, Is.False, code + ": " + evidence.Id + " page " + i);
                        Assert.That(Body.text, Is.Not.Empty);
                        foreach (char character in Body.text)
                            if (!char.IsControl(character)) Assert.That(Body.font.HasCharacter(character), Is.True, "Missing glyph " + character);
                        yield return Press(Key.Space);
                    }
                }
            }
            LogAssert.NoUnexpectedReceived();
        }
    }
}
