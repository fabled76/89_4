using System;
using UnityEngine;

namespace DeadAir
{
    [RequireComponent(typeof(BatteryComponent))]
    public sealed class RadioController : MonoBehaviour
    {
        [SerializeField] private bool startPoweredOn = true;
        [SerializeField, Min(0f)] private float drainPerSecond = 5f;
        private BatteryComponent battery;
        public bool IsOn { get; private set; }
        public event Action<bool> StateChanged;

        private void Awake() => battery = GetComponent<BatteryComponent>();
        private void OnEnable() => battery.Depleted += HandleDepleted;
        // Start runs after all Awake calls, including the battery's initialization.
        private void Start() => SetPower(startPoweredOn);
        private void Update() => Tick(Time.deltaTime);
        private void OnDisable()
        {
            battery.Depleted -= HandleDepleted;
            SetPower(false);
        }

        public void Toggle() => SetPower(!IsOn);
        public void SetPower(bool requestedOn)
        {
            bool next = requestedOn && isActiveAndEnabled && battery != null && battery.HasCharge;
            if (IsOn == next) return;
            IsOn = next;
            StateChanged?.Invoke(IsOn);
        }

        public void Tick(float seconds)
        {
            if (IsOn && seconds > 0f) battery.Consume(drainPerSecond * seconds);
        }

        private void HandleDepleted() => SetPower(false);
    }
}
