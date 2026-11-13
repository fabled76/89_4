using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadAir
{
    [RequireComponent(typeof(RadioController))]
    public sealed class RadioInput : MonoBehaviour
    {
        private RadioController radio;
        private InputAction toggle;
        private void Awake()
        {
            radio = GetComponent<RadioController>();
            toggle = new InputAction("ToggleRadio", InputActionType.Button, "<Keyboard>/leftShift");
        }
        private void OnEnable() => toggle.Enable();
        private void Update()
        {
            if (toggle.WasPressedThisFrame()) radio.Toggle();
        }
        private void OnDisable() => toggle.Disable();
        private void OnDestroy() => toggle.Dispose();
    }
}
