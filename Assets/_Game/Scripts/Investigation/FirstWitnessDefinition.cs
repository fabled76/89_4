using UnityEngine;

namespace DeadAir
{
    [CreateAssetMenu(menuName = "89.4/First Witness Scenario")]
    public sealed class FirstWitnessDefinition : ScriptableObject
    {
        [SerializeField] private string encounterTitle;
        [SerializeField, TextArea] private string emptyJournalMessage;
        [SerializeField] private EvidenceDefinition ledger;
        [SerializeField] private EvidenceDefinition photo;
        [SerializeField] private EvidenceDefinition testimony;
        [SerializeField, TextArea] private string exploreObjective;
        [SerializeField, TextArea] private string figureObjective;
        [SerializeField, TextArea] private string ledgerObjective;
        [SerializeField, TextArea] private string returnObjective;
        [SerializeField, TextArea] private string approachObjective;
        [SerializeField, TextArea] private string completedObjective;
        public EvidenceDefinition Ledger => ledger;
        public string EncounterTitle => PrototypeText.Get(encounterTitle);
        public string EmptyJournalMessage => PrototypeText.Get(emptyJournalMessage);
        public EvidenceDefinition Photo => photo;
        public EvidenceDefinition Testimony => testimony;
        public string ExploreObjective => PrototypeText.Get(exploreObjective);
        public string FigureObjective => PrototypeText.Get(figureObjective);
        public string LedgerObjective => PrototypeText.Get(ledgerObjective);
        public string ReturnObjective => PrototypeText.Get(returnObjective);
        public string ApproachObjective => PrototypeText.Get(approachObjective);
        public string CompletedObjective => PrototypeText.Get(completedObjective);
    }
}
