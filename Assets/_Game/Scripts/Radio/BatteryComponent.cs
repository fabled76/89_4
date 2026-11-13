using System;
using UnityEngine;

namespace DeadAir
{
    public sealed class BatteryComponent : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float capacity = 100f;
        [SerializeField, Range(0f, 1f)] private float startingFraction = 1f;
        public float Capacity => capacity;
        public float Charge { get; private set; }
        public float Fraction => capacity > 0f ? Charge / capacity : 0f;
        public bool HasCharge => Charge > 0f;
        public event Action<float> ChargeChanged;
        public event Action Depleted;

        private void Awake() => Charge = capacity * startingFraction;

        public void Consume(float amount)
        {
            if (amount <= 0f || float.IsNaN(amount) || !HasCharge) return;
            SetCharge(Mathf.Max(0f, Charge - amount));
        }

        public void ReplaceBattery() => SetCharge(capacity);

        private void SetCharge(float value)
        {
            if (Charge == value) return;
            bool wasCharged = HasCharge;
            Charge = value;
            // Enforce exhaustion before publishing the new UI value.
            if (wasCharged && !HasCharge) Depleted?.Invoke();
            ChargeChanged?.Invoke(Charge);
        }

        private void OnValidate() => capacity = Mathf.Max(0.01f, capacity);
    }
}
