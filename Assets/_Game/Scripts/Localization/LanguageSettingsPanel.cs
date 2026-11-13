using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    public sealed class LanguageSettingsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text headingText;
        [SerializeField] private TMP_Text optionsText;
        [SerializeField] private TMP_Text footerText;
        private InputAction openKey, englishKey, russianKey;

        private void Awake()
        {
            openKey = new InputAction("LanguageSettings", InputActionType.Button, "<Keyboard>/f1");
            englishKey = new InputAction("English", InputActionType.Button, "<Keyboard>/1");
            russianKey = new InputAction("Russian", InputActionType.Button, "<Keyboard>/2");
        }
        private void OnEnable()
        {
            openKey.Enable(); englishKey.Enable(); russianKey.Enable();
            LocalizationSettings.SelectedLocaleChanged += Refresh;
            panel.SetActive(false);
            StartCoroutine(RefreshAfterLocaleChange());
        }
        private IEnumerator RefreshAfterLocaleChange()
        {
            yield return LocalizationSettings.InitializationOperation;
            var locale = LocalizationSettings.SelectedLocale;
            headingText.text = PrototypeText.Get("settings.title");
            optionsText.text = PrototypeText.Get("settings.options");
            footerText.text = PrototypeText.Get("settings.footer", PrototypeText.Get("language." + locale.Identifier.Code));
        }
        private void OnDisable()
        {
            openKey.Disable(); englishKey.Disable(); russianKey.Disable();
            LocalizationSettings.SelectedLocaleChanged -= Refresh;
            StopAllCoroutines();
            if (panel != null) panel.SetActive(false);
        }
        private void OnDestroy()
        {
            openKey.Dispose(); englishKey.Dispose(); russianKey.Dispose();
        }
        private void Update()
        {
            if (openKey.WasPressedThisFrame()) panel.SetActive(!panel.activeSelf);
            if (!panel.activeSelf) return;
            if (englishKey.WasPressedThisFrame()) SelectLocale("en");
            if (russianKey.WasPressedThisFrame()) SelectLocale("ru");
        }
        public void SelectLocale(string code)
        {
            if (!PrototypeText.IsReady || (code != "en" && code != "ru")) return;
            var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
            if (locale == null) return;
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString(PrototypeText.LocalePreferenceKey, code);
            PlayerPrefs.Save();
            Refresh(locale);
        }
        private void Refresh(Locale _) => StartCoroutine(RefreshAfterLocaleChange());
    }
}
