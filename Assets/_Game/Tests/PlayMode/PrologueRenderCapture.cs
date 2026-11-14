using System.Collections;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeadAir.Tests
{
    // Actual URP camera renders for review. The temporary camera canvas is never saved to the scene.
    internal static class PrologueRenderCapture
    {
        public static IEnumerator Capture(string name, int width)
        {
            var camera = Camera.main;
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            var oldModes = new RenderMode[canvases.Length];
            var oldCameras = new Camera[canvases.Length];
            var oldDistances = new float[canvases.Length];
            var oldOrders = new int[canvases.Length];
            var target = new RenderTexture(width, width * 9 / 16, 24);
            var oldTarget = camera.targetTexture;
            var oldActive = RenderTexture.active;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            try
            {
                target.Create(); camera.targetTexture = target;
                for (int i = 0; i < canvases.Length; i++)
                {
                    oldModes[i] = canvases[i].renderMode; oldCameras[i] = canvases[i].worldCamera;
                    oldDistances[i] = canvases[i].planeDistance;
                    oldOrders[i] = canvases[i].sortingOrder;
                    if (oldModes[i] != RenderMode.ScreenSpaceOverlay) continue;
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera; canvases[i].planeDistance = 1;
                    canvases[i].sortingOrder = 30000 + i;
                }
                // Allow CanvasScaler and TMP rebuilds to settle after changing camera/target dimensions.
                yield return null; yield return null;
                Canvas.ForceUpdateCanvases();
                foreach (var text in Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
                {
                    text.ForceMeshUpdate();
                    Assert.That(text.isTextOverflowing, Is.False, name + "/" + width + "/" + text.name);
                }
                Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
                RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0); image.Apply();
                Directory.CreateDirectory("TestResults/PrologueCaptures");
                File.WriteAllBytes("TestResults/PrologueCaptures/" + name + "-" + width + ".png", image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
                for (int i = 0; i < canvases.Length; i++)
                {
                    canvases[i].renderMode = oldModes[i]; canvases[i].worldCamera = oldCameras[i];
                    canvases[i].planeDistance = oldDistances[i];
                    canvases[i].sortingOrder = oldOrders[i];
                }
                Object.Destroy(image); target.Release(); Object.Destroy(target);
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
