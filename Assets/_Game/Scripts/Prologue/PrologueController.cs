using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    public sealed class PrologueController : MonoBehaviour
    {
        [SerializeField] private PrologueDefinition definition;
        [SerializeField] private PrologueReadingPanel reader;
        [SerializeField] private RadioController radio;
        [SerializeField] private PlayerController2D movement;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private RadioInput radioInput;
        [SerializeField] private SpriteRenderer deskRadio;
        [SerializeField] private SpriteRenderer caseFolder;
        [SerializeField] private SpriteRenderer bag;
        [SerializeField] private GameObject journeyPrefab;
        [SerializeField] private TMP_Text objective;
        [SerializeField] private TMP_Text controls;
        [SerializeField] private TMP_Text prompt;
        [SerializeField] private bool tintPackedBag = true;
        public bool CallCompleted { get; private set; }
        public bool CaseReviewed { get; private set; }
        public bool BagPrepared { get; private set; }
        public bool DepartureStarted { get; private set; }
        public bool ReadyToPack => CallCompleted && CaseReviewed;
        public PrologueReadingPanel Reader => reader;
        public event Action Changed;
        public bool CanUse(PrologueInteraction action) => action != PrologueInteraction.Case || !BagPrepared;
        private void OnEnable()
        {
            radio.StateChanged += OnRadio;
            LocalizationSettings.SelectedLocaleChanged += OnLocale;
            StartCoroutine(RefreshLocale());
        }
        private void OnDisable()
        {
            radio.StateChanged -= OnRadio;
            LocalizationSettings.SelectedLocaleChanged -= OnLocale;
            StopAllCoroutines();
        }
        private void OnRadio(bool _) => Refresh();
        private void OnLocale(Locale _) => StartCoroutine(RefreshLocale());
        private IEnumerator RefreshLocale()
        {
            yield return LocalizationSettings.InitializationOperation;
            Refresh();
        }
        private void LateUpdate() => prompt.text = DepartureStarted ? string.Empty : interactor.CurrentPrompt;
        private void Notify() { Refresh(); Changed?.Invoke(); }
        private void Refresh()
        {
            if (!PrototypeText.IsReady) return;
            objective.text = BagPrepared ? definition.LeaveObjective : definition.Text(!CallCompleted
                ? "prologue.objective.answer_phone" : !CaseReviewed ? "prologue.objective.review_case" : "prologue.objective.prepare");
            controls.text = PrototypeText.Get("prologue.ui.controls", PrototypeText.Get(radio.IsOn ? "prologue.radio.on" : "prologue.radio.off"));
        }
        public void Interact(PrologueInteraction action)
        {
            if (DepartureStarted || reader.IsOpen) return;
            switch (action)
            {
                case PrologueInteraction.Phone:
                    reader.Show(definition.Call, () => { if (!CallCompleted) { CallCompleted = true; Notify(); } }); break;
                case PrologueInteraction.Case:
                    reader.Show(definition.CaseSummary, () => { if (!CaseReviewed) { CaseReviewed = true; Notify(); } }); break;
                case PrologueInteraction.Article: reader.Show(definition.Article); break;
                case PrologueInteraction.Radio: reader.Show(CaseReviewed ? definition.RadioAfterCase : definition.RadioBeforeCase); break;
                case PrologueInteraction.Bag:
                    if (BagPrepared) { reader.Show(definition.CaseSummary); break; }
                    if (!ReadyToPack) { reader.Hint(!CaseReviewed ? "prologue.bag.early" : "prologue.hint.call_required"); break; }
                    if (!BagPrepared)
                    {
                        BagPrepared = true; radio.SetPower(false); deskRadio.enabled = false; caseFolder.enabled = false;
                        if (tintPackedBag) bag.color = new Color(.3f, .65f, .4f);
                        Notify();
                    }
                    reader.Hint("prologue.bag.prepared"); break;
                case PrologueInteraction.Exit:
                    if (!BagPrepared)
                        reader.Hint(!CallCompleted ? "prologue.hint.call_required" : !CaseReviewed ? "prologue.hint.case_required" : "prologue.hint.pack_required");
                    else Depart();
                    break;
            }
        }
        private void Depart()
        {
            if (DepartureStarted) return;
            DepartureStarted = true; radio.SetPower(false);
            movement.enabled = false; interactor.enabled = false; radioInput.enabled = false;
            var languages = GetComponent<LanguageSettingsPanel>();
            if (languages != null) languages.enabled = false;
            var journey = Instantiate(journeyPrefab).GetComponent<PrologueJourney>();
            journey.Begin(definition); Notify();
        }
    }
}
