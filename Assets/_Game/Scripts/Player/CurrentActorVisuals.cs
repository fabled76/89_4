using UnityEngine;

namespace DeadAir
{
    // Artwork only: the physics root stays unflipped, including colliders and interaction reach.
    public sealed class CurrentActorVisuals : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Transform facingRoot;
        [SerializeField] private FirstNightController firstNight;
        [SerializeField] private GameObject carriedReceiver;
        private Vector3 originalScale;
        private void Awake() => originalScale = facingRoot.localScale;
        private void LateUpdate()
        {
            if (Mathf.Abs(body.linearVelocity.x) > .01f)
                facingRoot.localScale = new Vector3(originalScale.x * (body.linearVelocity.x > 0 ? 1 : -1), originalScale.y, originalScale.z);
            if (carriedReceiver != null) carriedReceiver.SetActive(firstNight != null && firstNight.HasReceiver);
        }
    }
}
