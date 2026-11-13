using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeadAir
{
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float radius = 1.8f;
        [SerializeField] private LayerMask interactionLayers = ~0;
        private readonly List<Collider2D> overlaps = new List<Collider2D>();
        private InputAction interact;
        public IInteractable CurrentTarget { get; private set; }
        public string CurrentPrompt => CurrentTarget?.Prompt ?? string.Empty;

        private void Awake() => interact = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
        private void OnEnable() => interact.Enable();
        private void OnDisable()
        {
            interact.Disable();
            CurrentTarget = null;
        }
        private void OnDestroy() => interact.Dispose();

        private void Update()
        {
            CurrentTarget = FindNearest();
            if (interact.WasPressedThisFrame() && CurrentTarget != null)
                CurrentTarget.Interact(this);
        }

        public IInteractable FindNearest()
        {
            var filter = new ContactFilter2D { useLayerMask = true, layerMask = interactionLayers, useTriggers = true };
            Physics2D.OverlapCircle(transform.position, radius, filter, overlaps);
            IInteractable nearest = null;
            float nearestDistance = float.PositiveInfinity;
            int nearestId = int.MaxValue;
            foreach (var collider in overlaps)
            {
                // The collider belongs to an interactable on itself or its parent.
                foreach (var behaviour in collider.GetComponentsInParent<MonoBehaviour>())
                {
                    if (!behaviour.isActiveAndEnabled || !(behaviour is IInteractable candidate)
                        || !candidate.CanInteract(this)) continue;
                    float distance = ((Vector2)transform.position - collider.ClosestPoint(transform.position)).sqrMagnitude;
                    int id = behaviour.GetInstanceID();
                    if (distance > nearestDistance || (distance == nearestDistance && id >= nearestId)) continue;
                    nearest = candidate;
                    nearestDistance = distance;
                    nearestId = id;
                }
            }
            return nearest;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
