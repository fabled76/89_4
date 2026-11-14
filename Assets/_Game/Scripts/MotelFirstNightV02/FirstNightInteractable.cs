using UnityEngine;

namespace DeadAir
{
    public enum FirstNightAction { CheckIn, GuestDoor, RetrieveReceiver, ReviewReceiver, CorridorDoor, ReturnToRoom, Man, LookAround, TestBattery }
    public enum FirstNightArea { Reception, GuestRoom, Corridor }

    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class FirstNightInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private FirstNightController controller;
        [SerializeField] private FirstNightAction action;
        [SerializeField] private string promptKey;
        public string Prompt => PrototypeText.Get(promptKey);
        public bool CanInteract(InteractionController interactor) => controller != null && controller.CanUse(action);
        public void Interact(InteractionController interactor) => controller.Use(action);
    }
}
