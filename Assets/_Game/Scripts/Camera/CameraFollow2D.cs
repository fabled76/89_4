using UnityEngine;

namespace DeadAir
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 worldMin = new Vector2(-15f, -2f);
        [SerializeField] private Vector2 worldMax = new Vector2(15f, 7f);
        [SerializeField, Min(0f)] private float followSpeed = 8f;
        private Camera view;
        private void Awake() => view = GetComponent<Camera>();
        private void Start() => Follow(true);
        private void LateUpdate() => Follow(false);

        private void Follow(bool snap)
        {
            if (target == null || !view.orthographic) return;
            float halfHeight = view.orthographicSize;
            float halfWidth = halfHeight * view.aspect;
            var desired = new Vector3(
                ClampAxis(target.position.x, worldMin.x, worldMax.x, halfWidth),
                ClampAxis(target.position.y, worldMin.y, worldMax.y, halfHeight),
                transform.position.z);
            transform.position = snap ? desired : Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSpeed * Time.deltaTime));
            // Re-clamp after smoothing for aspect/viewport changes.
            var position = transform.position;
            position.x = ClampAxis(position.x, worldMin.x, worldMax.x, halfWidth);
            position.y = ClampAxis(position.y, worldMin.y, worldMax.y, halfHeight);
            transform.position = position;
        }

        private static float ClampAxis(float value, float min, float max, float extent)
            => max - min <= extent * 2f ? (min + max) * 0.5f : Mathf.Clamp(value, min + extent, max - extent);
    }
}
