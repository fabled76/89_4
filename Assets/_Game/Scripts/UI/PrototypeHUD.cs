using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        [SerializeField] private RadioController radio;
        [SerializeField] private BatteryComponent battery;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text promptText;

        private void OnEnable()
        {
            if (radio == null || battery == null || statusText == null || promptText == null || interactor == null)
            {
                Debug.LogError("Assign all PrototypeHUD references. See the prototype setup guide.", this);
                enabled = false;
                return;
            }
            radio.StateChanged += OnRadioChanged;
            battery.ChargeChanged += OnChargeChanged;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            StartCoroutine(RefreshAfterLocaleChange());
        }
        private void OnDisable()
        {
            if (radio != null) radio.StateChanged -= OnRadioChanged;
            if (battery != null) battery.ChargeChanged -= OnChargeChanged;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            StopAllCoroutines();
        }
        private void LateUpdate() => promptText.text = interactor.CurrentPrompt;
        private void OnRadioChanged(bool _) => Refresh();
        private void OnChargeChanged(float _) => Refresh();
        private void OnLocaleChanged(Locale _) => StartCoroutine(RefreshAfterLocaleChange());
        private IEnumerator RefreshAfterLocaleChange()
        {
            yield return LocalizationSettings.InitializationOperation;
            Refresh();
        }
        private void Refresh()
        {
            if (!PrototypeText.IsReady) return;
            statusText.text = PrototypeText.Get("hud.status", PrototypeText.Get(radio.IsOn ? "radio.on" : "radio.off"), Mathf.RoundToInt(battery.Fraction * 100));
        }
    }
}
