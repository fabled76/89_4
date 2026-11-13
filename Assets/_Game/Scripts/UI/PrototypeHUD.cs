using UnityEngine;
using UnityEngine.UI;

namespace DeadAir
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        [SerializeField] private RadioController radio;
        [SerializeField] private BatteryComponent battery;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private Text statusText;
        [SerializeField] private Text promptText;

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
            Refresh();
        }
        private void OnDisable()
        {
            if (radio != null) radio.StateChanged -= OnRadioChanged;
            if (battery != null) battery.ChargeChanged -= OnChargeChanged;
        }
        private void LateUpdate() => promptText.text = interactor.CurrentPrompt;
        private void OnRadioChanged(bool _) => Refresh();
        private void OnChargeChanged(float _) => Refresh();
        private void Refresh()
        {
            statusText.text = $"89.4 — Motel prototype\nRADIO {(radio.IsOn ? "ON / STABLE" : "OFF / DRIFT")}  |  BATTERY {battery.Fraction:P0}\nA/D or arrows: walk   Left Shift: radio   E: interact";
        }
    }
}
