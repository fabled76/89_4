using UnityEngine;

namespace DeadAir.Studies
{
    [ExecuteAlways]
    public sealed class LabLightField : MonoBehaviour
    {
        public LabLightSource warm;
        public LabLightSource neon;
        [Range(0, 2)] public float ambient = .32f;
        [Range(.01f, 1)] public float minimumBrightness = .16f;
        public Renderer[] targets = new Renderer[0];
        private MaterialPropertyBlock properties;
        private static readonly int AmbientId = Shader.PropertyToID("_LabAmbient");
        private static readonly int MinimumId = Shader.PropertyToID("_LabMinimum");
        private static readonly int FlipId = Shader.PropertyToID("_LabSpriteFlip");
        private static readonly int[] WarmIds = { Shader.PropertyToID("_WarmPositionRange"), Shader.PropertyToID("_WarmColorIntensity"), Shader.PropertyToID("_WarmDirectionality") };
        private static readonly int[] NeonIds = { Shader.PropertyToID("_NeonPositionRange"), Shader.PropertyToID("_NeonColorIntensity"), Shader.PropertyToID("_NeonDirectionality") };

        private void OnEnable() => Apply();
        private void LateUpdate() => Apply();

        public void Apply()
        {
            if (targets == null) return;
            if (properties == null) properties = new MaterialPropertyBlock();
            foreach (var target in targets)
            {
                if (target == null) continue;
                target.GetPropertyBlock(properties);
                properties.SetFloat(AmbientId, ambient);
                properties.SetFloat(MinimumId, minimumBrightness);
                var sprite = target as SpriteRenderer;
                properties.SetVector(FlipId, new Vector4(sprite != null && sprite.flipX ? -1 : 1, sprite != null && sprite.flipY ? -1 : 1, 0, 0));
                WriteSource(properties, WarmIds, warm);
                WriteSource(properties, NeonIds, neon);
                target.SetPropertyBlock(properties);
            }
        }

        private static void WriteSource(MaterialPropertyBlock block, int[] ids, LabLightSource source)
        {
            Vector3 p = source != null ? source.transform.position : Vector3.zero;
            float radius = source != null ? source.range : 1;
            Color rgb = source != null ? source.color.linear : Color.black;
            float intensity = source != null ? source.EffectiveIntensity : 0;
            block.SetVector(ids[0], new Vector4(p.x, p.y, p.z, radius));
            // Explicit linear vectors avoid an additional color-property conversion.
            block.SetVector(ids[1], new Vector4(rgb.r, rgb.g, rgb.b, intensity));
            block.SetFloat(ids[2], source != null ? source.directionality : 0);
        }
    }
}
