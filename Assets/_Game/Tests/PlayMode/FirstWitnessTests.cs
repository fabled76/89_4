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
    public sealed class FirstWitnessTests
    {
        private Keyboard keyboard;
        private bool hadLocalePreference;
        private string localePreference;
        private InputSettings originalSettings, testSettings;
        private Rigidbody2D player;
        private RadioController radio;
        private BatteryComponent battery;
        private EvidenceJournal journal;
        private InvestigationStateTracker investigation;
        private InvestigationUI ui;
        private UnacceptedEncounter encounter;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            hadLocalePreference = PlayerPrefs.HasKey(PrototypeText.LocalePreferenceKey);
            localePreference = PlayerPrefs.GetString(PrototypeText.LocalePreferenceKey);
            originalSettings = InputSystem.settings;
            testSettings = Object.Instantiate(originalSettings);
            testSettings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            testSettings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = testSettings;
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(
                "Assets/_Game/Scenes/MotelCorridor.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            Assert.Ignore("Tests use the editor corridor asset.");
#endif
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            yield return null;
            keyboard = InputSystem.AddDevice<Keyboard>();
            yield return null;
            player = Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();
            radio = Object.FindFirstObjectByType<RadioController>();
            battery = Object.FindFirstObjectByType<BatteryComponent>();
            journal = Object.FindFirstObjectByType<EvidenceJournal>();
            investigation = Object.FindFirstObjectByType<InvestigationStateTracker>();
            ui = Object.FindFirstObjectByType<InvestigationUI>();
            encounter = Object.FindFirstObjectByType<UnacceptedEncounter>();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            InputSystem.settings = originalSettings;
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
        private IEnumerator MoveTo(float x)
        {
            player.position = new Vector2(x, 1);
            player.linearVelocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return null;
        }
        private IEnumerator FinishReading()
        {
            Assert.That(ui.ActiveEvidence, Is.Not.Null);
            int pages = ui.ActiveEvidence.PageCount;
            for (int i = 0; i < pages; i++) yield return Press(Key.D);
            Assert.That(ui.IsOpen, Is.False);
        }
        private IEnumerator ReadLedger()
        {
            yield return MoveTo(-5);
            yield return Press(Key.E);
            yield return FinishReading();
            Assert.That(investigation.LedgerRead, Is.True);
        }

        [UnityTest]
        public IEnumerator EvidencePagesFitTheReadingAndJournalPanel()
        {
            var body = System.Array.Find(GameObject.Find("PrototypeHUD").GetComponentsInChildren<TMP_Text>(true), text => text.name == "Body");
            foreach (var evidence in Resources.FindObjectsOfTypeAll<EvidenceDefinition>())
            for (int pageIndex = 0; pageIndex < evidence.PageCount; pageIndex++)
            {
                body.text = evidence.GetPage(pageIndex);
                Canvas.ForceUpdateCanvases();
                body.ForceMeshUpdate(true);
                Assert.That(body.isTextOverflowing, Is.False, evidence.Title + " overflows reading/journal body.");
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator FullEncounterThroughKeyboardAndTriggerAddsStationLead()
        {
            Assert.That(journal.Entries.Count, Is.Zero);
            Assert.That(encounter.State, Is.EqualTo(UnacceptedState.Hidden));
            yield return MoveTo(7);
            yield return Press(Key.LeftShift);
            Assert.That(investigation.SeenUnaccepted, Is.True);
            Assert.That(encounter.State, Is.EqualTo(UnacceptedState.Visible));
            Assert.That(ui.IsOpen, Is.False, "Approach before the ledger must not deliver testimony.");
            yield return Press(Key.LeftShift);
            Assert.That(investigation.SuppressedUnaccepted, Is.True);

            yield return MoveTo(-5);
            yield return Press(Key.E);
            Assert.That(player.GetComponent<PlayerController2D>().enabled, Is.False);
            yield return Press(Key.Escape);
            Assert.That(investigation.LedgerRead, Is.False, "Abandoning reading must be retryable.");
            yield return Press(Key.E);
            yield return FinishReading();
            Assert.That(investigation.LedgerRead, Is.True);
            Assert.That(journal.Entries.Count, Is.EqualTo(1));

            yield return MoveTo(3);
            yield return Press(Key.LeftShift);
            Assert.That(investigation.VoluntaryReturn, Is.True);
            Assert.That(encounter.State, Is.EqualTo(UnacceptedState.AwaitingApproach));
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return new WaitForSeconds(1f);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            Assert.That(ui.IsOpen, Is.True, "Walking into the trigger should begin testimony.");
            yield return FinishReading();
            Assert.That(investigation.TestimonyReceived, Is.True);
            Assert.That(journal.Entries.Count, Is.EqualTo(2));
            Assert.That(encounter.State, Is.EqualTo(UnacceptedState.TestimonyDelivered));
            Assert.That(investigation.Objective, Does.Contain("radio station"));

            yield return Press(Key.Tab);
            Assert.That(ui.IsOpen, Is.True);
            yield return Press(Key.D);
            yield return Press(Key.Tab);
            Assert.That(ui.IsOpen, Is.False);
            yield return Press(Key.Tab);
            Assert.That(ui.IsOpen, Is.True);
            yield return Press(Key.Escape);
            Assert.That(player.GetComponent<PlayerController2D>().enabled, Is.True);
            Assert.That(player.GetComponent<InteractionController>().enabled, Is.True);
            Assert.That(journal.Entries.Count, Is.EqualTo(2));
            yield return Press(Key.LeftShift);
            yield return Press(Key.LeftShift);
            yield return new WaitForFixedUpdate();
            Assert.That(ui.IsOpen, Is.False, "A completed encounter must not repeatedly open testimony.");
            Assert.That(journal.Entries.Count, Is.EqualTo(2));
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator ExhaustionDoesNotCountAsChoiceAndRadioCanInterruptTestimony()
        {
            yield return ReadLedger();
            yield return MoveTo(7);
            battery.Consume(battery.Capacity);
            yield return new WaitForFixedUpdate();
            Assert.That(investigation.VoluntaryReturn, Is.False);
            Assert.That(ui.IsOpen, Is.False);
            battery.ReplaceBattery();
            yield return Press(Key.LeftShift);
            yield return Press(Key.LeftShift);
            yield return new WaitForFixedUpdate();
            Assert.That(ui.IsOpen, Is.True, "Toggling off while already in range should also work.");
            yield return Press(Key.D);
            yield return Press(Key.LeftShift);
            Assert.That(ui.IsOpen, Is.False);
            Assert.That(investigation.TestimonyReceived, Is.False);
            Assert.That(encounter.State, Is.EqualTo(UnacceptedState.Hidden));
            Assert.That(player.GetComponent<PlayerController2D>().enabled, Is.True);
            yield return Press(Key.LeftShift);
            yield return new WaitForFixedUpdate();
            Assert.That(ui.IsOpen, Is.True);
            yield return FinishReading();
            Assert.That(investigation.TestimonyReceived, Is.True);
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator LedgerReadWhileOffRequiresNewChoiceAndPhotoNeverDuplicates()
        {
            yield return Press(Key.LeftShift);
            yield return ReadLedger();
            yield return MoveTo(7);
            Assert.That(ui.IsOpen, Is.False);
            Assert.That(investigation.VoluntaryReturn, Is.False);
            yield return Press(Key.LeftShift);
            yield return Press(Key.LeftShift);
            yield return new WaitForFixedUpdate();
            Assert.That(ui.IsOpen, Is.True);
            yield return Press(Key.Escape);
            yield return new WaitForSeconds(.1f);
            Assert.That(ui.IsOpen, Is.False, "Esc must not cause a testimony reopen loop.");
            yield return MoveTo(3);
            yield return MoveTo(7);
            Assert.That(ui.IsOpen, Is.True, "Leaving and returning should allow a retry.");
            yield return FinishReading();
            yield return MoveTo(-13);
            yield return Press(Key.E);
            yield return FinishReading();
            yield return Press(Key.E);
            yield return FinishReading();
            Assert.That(journal.Entries.Count, Is.EqualTo(3));
            yield return Press(Key.LeftShift);
            Assert.That(journal.Entries.Count, Is.EqualTo(3), "Changing world presentation cannot erase evidence.");
            LogAssert.NoUnexpectedReceived();
        }
    }
}
