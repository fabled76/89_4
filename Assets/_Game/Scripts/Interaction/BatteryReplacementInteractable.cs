using UnityEngine;

namespace DeadAir
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class BatteryReplacementInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private BatteryComponent battery;
        public string Prompt => PrototypeText.Get("prompt.battery");
        public bool CanInteract(InteractionController interactor) => battery != null;
        public void Interact(InteractionController interactor)
        {
            if (battery == null) return;
            battery.ReplaceBattery();
            Debug.Log("Test battery replaced. Use Left Shift to power the radio if it is off.", this);
        }
    }
}
