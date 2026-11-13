using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    public sealed class InvestigationUI : MonoBehaviour
    {
        [SerializeField] private EvidenceJournal journal;
        [SerializeField] private InvestigationStateTracker investigation;
        [SerializeField] private PlayerController2D movement;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text headingText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text footerText;
        [SerializeField] private TMP_Text objectiveText;
        private InputAction journalKey, nextKey, previousKey, closeKey;
        private readonly List<(EvidenceDefinition evidence, int page)> journalPages = new List<(EvidenceDefinition, int)>();
        private EvidenceDefinition currentEvidence;
        private Action onCompleted;
        private int pageIndex;
        private bool journalMode, restoreMovement, restoreInteraction;
        public bool IsOpen { get; private set; }
        public EvidenceDefinition ActiveEvidence => currentEvidence;
        public event Action<EvidenceDefinition, bool> DialogueClosed;

        private void Awake()
        {
            journalKey = new InputAction("Journal", InputActionType.Button, "<Keyboard>/tab");
            nextKey = new InputAction("NextPage", InputActionType.Button, "<Keyboard>/space");
            nextKey.AddBinding("<Keyboard>/enter");
            nextKey.AddBinding("<Keyboard>/rightArrow");
            previousKey = new InputAction("PreviousPage", InputActionType.Button, "<Keyboard>/leftArrow");
            closeKey = new InputAction("ClosePanel", InputActionType.Button, "<Keyboard>/escape");
        }
        private void OnEnable()
        {
            journalKey.Enable(); nextKey.Enable(); previousKey.Enable(); closeKey.Enable();
            journal.Changed += RefreshJournal;
            investigation.Changed += RefreshObjective;
            LocalizationSettings.SelectedLocaleChanged += RefreshLocale;
            if (panel != null) panel.SetActive(false);
            StartCoroutine(RefreshAfterLocaleChange());
        }
        private void OnDisable()
        {
            journalKey.Disable(); nextKey.Disable(); previousKey.Disable(); closeKey.Disable();
            journal.Changed -= RefreshJournal;
            investigation.Changed -= RefreshObjective;
            LocalizationSettings.SelectedLocaleChanged -= RefreshLocale;
            StopAllCoroutines();
            Close(false);
        }
        private void OnDestroy()
        {
            journalKey.Dispose(); nextKey.Dispose(); previousKey.Dispose(); closeKey.Dispose();
        }
        private IEnumerator RefreshAfterLocaleChange()
        {
            yield return LocalizationSettings.InitializationOperation;
            RefreshObjective();
            if (IsOpen) RenderPage();
        }
        private void RefreshLocale(Locale _) => StartCoroutine(RefreshAfterLocaleChange());

        private void Update()
        {
            if (closeKey.WasPressedThisFrame() && IsOpen) { Close(false); return; }
            if (journalKey.WasPressedThisFrame())
            {
                // Do not silently abandon an unfinished testimony by opening the journal.
                if (!IsOpen) OpenJournal();
                else if (journalMode) Close(false);
                return;
            }
            if (!IsOpen) return;
            if (previousKey.WasPressedThisFrame()) { pageIndex = Mathf.Max(0, pageIndex - 1); RenderPage(); }
            if (nextKey.WasPressedThisFrame())
            {
                int count = journalMode ? journalPages.Count : currentEvidence.PageCount;
                if (pageIndex + 1 < count) { pageIndex++; RenderPage(); }
                else if (!journalMode) Close(true);
            }
        }

        public bool ShowEvidence(EvidenceDefinition evidence, Action completed)
        {
            if (IsOpen || evidence == null || evidence.PageCount == 0) return false;
            currentEvidence = evidence;
            onCompleted = completed;
            journalMode = false;
            Open();
            return true;
        }

        public void CancelEvidence(EvidenceDefinition evidence)
        {
            if (IsOpen && !journalMode && currentEvidence == evidence) Close(false);
        }

        public void OpenJournal()
        {
            if (IsOpen) return;
            journalMode = true;
            BuildJournalPages();
            Open();
        }

        private void Open()
        {
            IsOpen = true;
            pageIndex = 0;
            restoreMovement = movement.enabled;
            restoreInteraction = interactor.enabled;
            movement.enabled = false;
            interactor.enabled = false;
            panel.SetActive(true);
            RenderPage();
        }

        private void Close(bool completed)
        {
            if (!IsOpen) return;
            var evidence = currentEvidence;
            var callback = onCompleted;
            bool wasDialogue = !journalMode;
            IsOpen = false;
            currentEvidence = null;
            onCompleted = null;
            if (panel != null) panel.SetActive(false);
            if (movement != null) movement.enabled = restoreMovement;
            if (interactor != null) interactor.enabled = restoreInteraction;
            if (wasDialogue)
            {
                // Notify before publishing evidence so trigger re-entry cannot race completion.
                DialogueClosed?.Invoke(evidence, completed);
                if (completed) callback?.Invoke();
            }
        }

        private void RenderPage()
        {
            if (!PrototypeText.IsReady) return;
            if (journalMode)
            {
                var entry = journalPages[pageIndex];
                headingText.text = PrototypeText.Get("ui.journal.heading", entry.evidence != null ? entry.evidence.Title : PrototypeText.Get("ui.journal.emptyTitle"));
                bodyText.text = entry.evidence != null ? entry.evidence.GetPage(entry.page) : investigation.EmptyJournalMessage;
                footerText.text = PrototypeText.Get("ui.journal.footer", journal.Entries.Count, pageIndex + 1, journalPages.Count);
            }
            else
            {
                headingText.text = currentEvidence.Title;
                bodyText.text = currentEvidence.GetPage(pageIndex);
                footerText.text = PrototypeText.Get("ui.reading.footer", pageIndex + 1, currentEvidence.PageCount);
            }
        }

        private void BuildJournalPages()
        {
            journalPages.Clear();
            foreach (var evidence in journal.Entries)
                for (int page = 0; page < evidence.PageCount; page++) journalPages.Add((evidence, page));
            if (journalPages.Count == 0) journalPages.Add((null, 0));
        }
        private void RefreshJournal()
        {
            if (!IsOpen || !journalMode) return;
            BuildJournalPages();
            pageIndex = Mathf.Clamp(pageIndex, 0, journalPages.Count - 1);
            RenderPage();
        }
        private void RefreshObjective()
        {
            if (PrototypeText.IsReady)
                objectiveText.text = PrototypeText.Get("ui.objective", investigation.EncounterTitle, investigation.Objective);
        }
    }
}
