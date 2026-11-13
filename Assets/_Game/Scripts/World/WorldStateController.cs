using System;
using UnityEngine;

namespace DeadAir
{
    public enum WorldState { Stable, Drift }

    public sealed class WorldStateController : MonoBehaviour
    {
        [SerializeField] private RadioController radio;
        [Tooltip("Visuals only. Keep shared collision and all controllers outside these roots.")]
        [SerializeField] private GameObject stableVisuals;
        [SerializeField] private GameObject driftVisuals;
        public WorldState State { get; private set; } = WorldState.Drift;
        public event Action<WorldState> StateChanged;

        private void OnEnable()
        {
            if (radio == null)
            {
                Debug.LogError("Assign the local portable radio to WorldStateController.", this);
                enabled = false;
                return;
            }
            radio.StateChanged += ApplyRadioState;
            ApplyRadioState(radio.IsOn);
        }

        private void OnDisable()
        {
            if (radio != null) radio.StateChanged -= ApplyRadioState;
        }

        private void ApplyRadioState(bool isOn)
        {
            var next = isOn ? WorldState.Stable : WorldState.Drift;
            bool changed = State != next;
            State = next;
            if (stableVisuals != null) stableVisuals.SetActive(isOn);
            if (driftVisuals != null) driftVisuals.SetActive(!isOn);
            if (changed) StateChanged?.Invoke(State);
        }
    }
}
