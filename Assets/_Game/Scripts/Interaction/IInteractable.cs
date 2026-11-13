using UnityEngine;

namespace DeadAir
{
    public interface IInteractable
    {
        string Prompt { get; }
        bool CanInteract(InteractionController interactor);
        void Interact(InteractionController interactor);
    }
}
