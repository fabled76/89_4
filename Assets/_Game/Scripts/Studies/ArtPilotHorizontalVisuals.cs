using UnityEngine;

namespace DeadAir
{
    // Preprojected images follow only X; physics settling must not shift their camera registration.
    public sealed class ArtPilotHorizontalVisuals : MonoBehaviour
    {
        [SerializeField] private Transform body;
        [SerializeField] private float referenceBodyX = .9f;
        [SerializeField] private Vector3 frameCenter = new Vector3(0, 1.2f, .25f);

        private void LateUpdate()
        {
            if (body != null) transform.position = frameCenter + Vector3.right * (body.position.x - referenceBodyX);
        }
    }
}
