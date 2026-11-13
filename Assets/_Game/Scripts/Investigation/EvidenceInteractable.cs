using UnityEngine;

namespace DeadAir
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class EvidenceInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private EvidenceDefinition evidence;
        [SerializeField] private EvidenceJournal journal;
        [SerializeField] private InvestigationUI presentation;
        [SerializeField, Tooltip("Prototype String Table key.")] private string prompt = "prompt.inspect";
        public string Prompt => PrototypeText.Get(prompt);
        public bool CanInteract(InteractionController interactor) => evidence != null && journal != null
            && presentation != null && !presentation.IsOpen;
        public void Interact(InteractionController interactor)
        {
            if (CanInteract(interactor)) presentation.ShowEvidence(evidence, () => journal.Add(evidence));
        }
    }
}
