using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadAir.Studies
{
    public sealed class LabReviewControls : MonoBehaviour
    {
        public LabLightField field;
        public Rigidbody2D actor;
        public Vector2 resetPosition = new Vector2(.35f, .92f);
        private InputAction warmKey, neonKey, ambientKey, resetKey;

        private void Awake()
        {
            warmKey = new InputAction("LabWarm", InputActionType.Button, "<Keyboard>/1");
            neonKey = new InputAction("LabNeon", InputActionType.Button, "<Keyboard>/2");
            ambientKey = new InputAction("LabAmbient", InputActionType.Button, "<Keyboard>/h");
            resetKey = new InputAction("LabReset", InputActionType.Button, "<Keyboard>/r");
        }
        private void OnEnable() { warmKey.Enable(); neonKey.Enable(); ambientKey.Enable(); resetKey.Enable(); }
        private void OnDisable() { warmKey.Disable(); neonKey.Disable(); ambientKey.Disable(); resetKey.Disable(); }
        private void OnDestroy() { warmKey.Dispose(); neonKey.Dispose(); ambientKey.Dispose(); resetKey.Dispose(); }
        private void Update()
        {
            if (warmKey.WasPressedThisFrame()) field.warm.isOn = !field.warm.isOn;
            if (neonKey.WasPressedThisFrame()) field.neon.isOn = !field.neon.isOn;
            if (ambientKey.WasPressedThisFrame()) field.ambient = field.ambient < .7f ? 1 : .32f;
            if (resetKey.WasPressedThisFrame()) ResetActor();
        }
        public void ResetActor() { actor.position = resetPosition; actor.linearVelocity = Vector2.zero; }
    }
}
