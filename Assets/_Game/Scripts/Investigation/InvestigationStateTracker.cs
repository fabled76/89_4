using System;
using UnityEngine;

namespace DeadAir
{
    public sealed class InvestigationStateTracker : MonoBehaviour
    {
        [SerializeField] private EvidenceJournal journal;
        [SerializeField] private FirstWitnessDefinition scenario;
        public bool SeenUnaccepted { get; private set; }
        public bool SuppressedUnaccepted { get; private set; }
        public bool VoluntaryReturn { get; private set; }
        public bool LedgerRead => journal.Contains(scenario.Ledger);
        public bool TestimonyReceived => journal.Contains(scenario.Testimony);
        public string EncounterTitle => scenario.EncounterTitle;
        public string EmptyJournalMessage => scenario.EmptyJournalMessage;
        public event Action Changed;

        public string Objective => TestimonyReceived ? scenario.CompletedObjective
            : LedgerRead ? (VoluntaryReturn ? scenario.ApproachObjective : scenario.ReturnObjective)
            : SuppressedUnaccepted ? scenario.LedgerObjective
            : SeenUnaccepted ? scenario.FigureObjective : scenario.ExploreObjective;

        private void OnEnable() => journal.Changed += NotifyChanged;
        private void OnDisable() => journal.Changed -= NotifyChanged;
        private void NotifyChanged() => Changed?.Invoke();
        public void MarkSeen()
        {
            if (SeenUnaccepted) return;
            SeenUnaccepted = true;
            NotifyChanged();
        }
        public void MarkSuppressed()
        {
            if (!SeenUnaccepted || SuppressedUnaccepted) return;
            SuppressedUnaccepted = true;
            NotifyChanged();
        }
        public void SetVoluntaryReturn(bool value)
        {
            if (VoluntaryReturn == value) return;
            VoluntaryReturn = value;
            NotifyChanged();
        }
    }
}
