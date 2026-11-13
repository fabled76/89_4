using UnityEngine;

namespace DeadAir
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class InspectInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField, Tooltip("Prototype String Table key.")] private string prompt = "prompt.marker";
        [SerializeField, TextArea] private string message = "Prototype interaction works. No story fact established.";
        public string Prompt => PrototypeText.Get(prompt);
        public bool CanInteract(InteractionController interactor) => true;
        public void Interact(InteractionController interactor) => Debug.Log(message, this);
    }
}
