using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    // Same reading controls/lock behavior as InvestigationUI, without motel journal/quest dependencies.
    public sealed class PrologueReadingPanel : MonoBehaviour
    {
        [SerializeField, Tooltip("Optional prologue name formatting; leave unset for independent trial documents.")] private PrologueDefinition definition;
        [SerializeField] private PlayerController2D movement;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text heading;
        [SerializeField] private TMP_Text body;
        [SerializeField] private TMP_Text footer;
        private InputAction next, previous, close;
        private Action completion;
        private int page;
        private string hintKey;
        private bool restoreMovement, restoreInteraction;
        private bool navigationReady, controlsLocked;
        private Coroutine restoreRoutine;
        public EvidenceDefinition ActiveDocument { get; private set; }
        public bool IsOpen { get; private set; }
        public event Action<EvidenceDefinition> Opened;
        public event Action<EvidenceDefinition, bool> Closed;

        private void Awake()
        {
            next = new InputAction("AdvanceReading", InputActionType.Button, "<Keyboard>/d");
            previous = new InputAction("PreviousReading", InputActionType.Button, "<Keyboard>/a");
            close = new InputAction("CloseReading", InputActionType.Button, "<Keyboard>/escape");
        }
        private void OnEnable()
        {
            next.Enable(); previous.Enable(); close.Enable();
            LocalizationSettings.SelectedLocaleChanged += OnLocale;
            panel.SetActive(false);
        }
        private void OnDisable()
        {
            next.Disable(); previous.Disable(); close.Disable();
            LocalizationSettings.SelectedLocaleChanged -= OnLocale;
            StopAllCoroutines(); restoreRoutine = null; Dismiss(false); RestoreControls();
        }
        private void OnDestroy() { next.Dispose(); previous.Dispose(); close.Dispose(); }
        private void OnLocale(Locale _) => StartCoroutine(RefreshLocale());
        private IEnumerator RefreshLocale()
        {
            yield return LocalizationSettings.InitializationOperation;
            if (IsOpen) Render();
        }
        private void Update()
        {
            if (!IsOpen) return;
            if (close.WasPressedThisFrame()) { Dismiss(false); return; }
            if (!navigationReady) { navigationReady = ReadingNavigation.KeysReleased; return; }
            if (previous.WasPressedThisFrame()) { page = Mathf.Max(0, page - 1); Render(); }
            if (!next.WasPressedThisFrame()) return;
            int count = ActiveDocument != null ? ActiveDocument.PageCount : 1;
            if (++page >= count) Dismiss(true); else Render();
        }
        public bool Show(EvidenceDefinition document, Action completed = null)
        {
            if (IsOpen || !PrototypeText.IsReady || document == null || document.PageCount == 0) return false;
            ActiveDocument = document; hintKey = null; completion = completed;
            Open(); return true;
        }
        public void Hint(string key)
        {
            if (IsOpen || !PrototypeText.IsReady) return;
            ActiveDocument = null; hintKey = key; completion = null; Open();
        }
        private void Open()
        {
            IsOpen = true; page = 0; navigationReady = ReadingNavigation.KeysReleased;
            if (restoreRoutine != null) { StopCoroutine(restoreRoutine); restoreRoutine = null; }
            if (!controlsLocked) { restoreMovement = movement.enabled; restoreInteraction = interactor.enabled; }
            controlsLocked = true;
            movement.enabled = false; interactor.enabled = false;
            panel.SetActive(true); Render(); Opened?.Invoke(ActiveDocument);
        }
        public void Dismiss(bool completed)
        {
            if (!IsOpen) return;
            var document = ActiveDocument; var callback = completion;
            IsOpen = false; ActiveDocument = null; completion = null;
            if (panel != null) panel.SetActive(false);
            if (isActiveAndEnabled && !ReadingNavigation.KeysReleased) restoreRoutine = StartCoroutine(RestoreAfterRelease());
            else RestoreControls();
            Closed?.Invoke(document, completed);
            if (completed) callback?.Invoke();
        }
        private IEnumerator RestoreAfterRelease()
        {
            yield return new WaitUntil(() => ReadingNavigation.KeysReleased);
            restoreRoutine = null;
            if (!IsOpen) RestoreControls();
        }
        private void RestoreControls()
        {
            if (!controlsLocked) return;
            controlsLocked = false;
            if (movement != null) movement.enabled = restoreMovement;
            if (interactor != null) interactor.enabled = restoreInteraction;
        }
        private void Render()
        {
            if (!PrototypeText.IsReady) return;
            string title = ActiveDocument != null ? ActiveDocument.Title : PrototypeText.Get("prologue.ui.title");
            string content = ActiveDocument != null ? ActiveDocument.GetPage(page) : PrototypeText.Get(hintKey);
            heading.text = definition != null ? definition.Format(title) : title;
            body.text = definition != null ? definition.Format(content) : content;
            footer.text = PrototypeText.Get("ui.reading.footer", page + 1, ActiveDocument != null ? ActiveDocument.PageCount : 1);
        }
    }
}
