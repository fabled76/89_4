using System;
using UnityEngine;

namespace DeadAir
{
    public enum UnacceptedState { Hidden, Visible, AwaitingApproach, TestimonyDelivered }

    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class UnacceptedEncounter : MonoBehaviour
    {
        [SerializeField] private RadioController radio;
        [SerializeField] private BatteryComponent battery;
        [SerializeField] private InvestigationStateTracker investigation;
        [SerializeField] private EvidenceJournal journal;
        [SerializeField] private FirstWitnessDefinition scenario;
        [SerializeField] private InvestigationUI presentation;
        [SerializeField] private Rigidbody2D playerBody;
        [SerializeField, Min(0f)] private float noticeDistance = 8f;
        private bool retryAfterExit;
        public UnacceptedState State { get; private set; }
        public event Action<UnacceptedState> StateChanged;

        private void OnEnable()
        {
            radio.StateChanged += OnRadioChanged;
            investigation.Changed += Refresh;
            presentation.DialogueClosed += OnDialogueClosed;
            Refresh();
        }
        private void OnDisable()
        {
            radio.StateChanged -= OnRadioChanged;
            investigation.Changed -= Refresh;
            presentation.DialogueClosed -= OnDialogueClosed;
            presentation.CancelEvidence(scenario.Testimony);
            investigation.SetVoluntaryReturn(false);
        }

        private void Update()
        {
            if (!radio.IsOn && Vector2.Distance(playerBody.position, transform.position) <= noticeDistance)
                investigation.MarkSeen();
        }

        private void OnRadioChanged(bool poweredOn)
        {
            if (poweredOn)
            {
                investigation.MarkSuppressed();
                investigation.SetVoluntaryReturn(false);
                presentation.CancelEvidence(scenario.Testimony);
            }
            else
            {
                // Exhaustion has already reduced charge to zero before this event fires.
                investigation.SetVoluntaryReturn(radio.isActiveAndEnabled && battery.HasCharge && investigation.LedgerRead);
                retryAfterExit = false;
            }
            Refresh();
        }

        private void Refresh()
        {
            var next = radio.IsOn ? UnacceptedState.Hidden
                : investigation.TestimonyReceived ? UnacceptedState.TestimonyDelivered
                : investigation.LedgerRead && investigation.VoluntaryReturn ? UnacceptedState.AwaitingApproach
                : UnacceptedState.Visible;
            if (State == next) return;
            State = next;
            StateChanged?.Invoke(State);
        }

        private void OnTriggerEnter2D(Collider2D other) => TryApproach(other);
        private void OnTriggerStay2D(Collider2D other) => TryApproach(other);
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.attachedRigidbody == playerBody) retryAfterExit = false;
        }

        private void TryApproach(Collider2D other)
        {
            if (other.attachedRigidbody != playerBody || retryAfterExit || presentation.IsOpen
                || State != UnacceptedState.AwaitingApproach) return;
            presentation.ShowEvidence(scenario.Testimony, CompleteTestimony);
        }

        private void CompleteTestimony()
        {
            // Recheck eligibility at completion; a suppressed witness cannot finish speaking.
            if (!radio.IsOn && investigation.LedgerRead && investigation.VoluntaryReturn)
                journal.Add(scenario.Testimony);
        }

        private void OnDialogueClosed(EvidenceDefinition evidence, bool completed)
        {
            if (evidence == scenario.Testimony) retryAfterExit = true;
        }
    }
}
