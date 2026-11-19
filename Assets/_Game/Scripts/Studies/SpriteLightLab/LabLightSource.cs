using UnityEngine;

namespace DeadAir.Studies
{
    // Authored light-field parameters, deliberately not a native URP/3D Light.
    public sealed class LabLightSource : MonoBehaviour
    {
        public bool isOn = true;
        [ColorUsage(false)] public Color color = Color.white;
        [Min(0)] public float intensity = 1;
        [Min(.01f)] public float range = 2.5f;
        [Range(0, 1)] public float directionality = .8f;
        public float EffectiveIntensity => isActiveAndEnabled && isOn ? intensity : 0;

        public static float Falloff(float distance, float radius)
        {
            float t = Mathf.Clamp01(1 - distance / Mathf.Max(.01f, radius));
            return t * t * (3 - 2 * t);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = color; Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
