using UnityEngine;

namespace DeadAir
{
    // Study-only reversible visibility check; no possession, inventory or narrative state.
    public sealed class ArtPilotReceiverToggle : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject receiverVisual;
        [SerializeField, Tooltip("Assign only the designer's removable receiver-shadow contribution, never the character shadow or baked room lighting.")] private GameObject receiverShadow;
        public bool ReceiverVisible { get; private set; } = true;
        public string Prompt => string.Empty; // Silent study: E uses the existing proximity query, without a HUD.
        public bool CanInteract(InteractionController _) => receiverVisual != null;
        public void Interact(InteractionController _) => SetVisible(!ReceiverVisible);

        public void SetVisible(bool visible)
        {
            ReceiverVisible = visible;
            if (receiverVisual != null) receiverVisual.SetActive(visible);
            if (receiverShadow != null) receiverShadow.SetActive(visible);
        }
    }
}
