using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeadAir.Tests
{
    internal static class ArtPaletteCPilotCapture
    {
        public static IEnumerator Save(string name, int width)
        {
            yield return null; yield return null;
            var camera = Camera.main;
            var target = new RenderTexture(width, width * 9 / 16, 24);
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float oldAspect = camera.aspect;
            var png = new Texture2D(width, target.height, TextureFormat.RGB24, false);
            try
            {
                target.Create(); camera.aspect = 16f / 9f; camera.targetTexture = target;
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
                RenderTexture.active = target; png.ReadPixels(new Rect(0, 0, width, target.height), 0, 0); png.Apply();
                Directory.CreateDirectory("TestResults/ArtPaletteCPilot/v03");
                File.WriteAllBytes("TestResults/ArtPaletteCPilot/v03/" + name + "-" + width + ".png", png.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = oldTarget; camera.aspect = oldAspect; RenderTexture.active = oldActive;
                Object.Destroy(png); target.Release(); Object.Destroy(target);
            }
        }
    }
}
