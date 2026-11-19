using UnityEngine;

namespace DeadAir.Studies
{
    [ExecuteAlways]
    public sealed class LabContactOval : MonoBehaviour
    {
        public Transform actor;
        [Min(.01f)] public float radius = .22f;
        [Range(.1f, 1)] public float depthAspect = .55f;
        [Range(0, .6f)] public float opacity = .24f;
        public Vector3 floorOffset = new Vector3(0, .006f, .025f);
        public float floorY;
        private MaterialPropertyBlock properties;

        private void OnEnable() => Apply();
        private void LateUpdate() => Apply();
        public void Apply()
        {
            if (actor == null) return;
            transform.SetPositionAndRotation(new Vector3(actor.position.x + floorOffset.x, floorY + floorOffset.y, actor.position.z + floorOffset.z), Quaternion.identity);
            transform.localScale = new Vector3(radius, 1, radius * depthAspect);
            if (properties == null) properties = new MaterialPropertyBlock();
            var renderer = GetComponent<Renderer>();
            if (renderer == null) return;
            renderer.GetPropertyBlock(properties); properties.SetFloat("_Opacity", opacity); renderer.SetPropertyBlock(properties);
        }
    }
}
