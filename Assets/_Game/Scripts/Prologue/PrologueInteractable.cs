using UnityEngine;

namespace DeadAir
{
    public enum PrologueInteraction { Phone, Case, Article, Radio, Bag, Exit }

    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PrologueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private PrologueController controller;
        [SerializeField] private PrologueInteraction interaction;
        [SerializeField] private string promptKey;
        public string Prompt => PrototypeText.Get(interaction == PrologueInteraction.Bag && controller.BagPrepared ? "prologue.prompt.bag_review" : promptKey);
        public bool CanInteract(InteractionController _) => PrototypeText.IsReady && !controller.DepartureStarted && !controller.Reader.IsOpen && controller.CanUse(interaction);
        public void Interact(InteractionController interactor)
        {
            if (CanInteract(interactor)) controller.Interact(interaction);
        }
    }
}
