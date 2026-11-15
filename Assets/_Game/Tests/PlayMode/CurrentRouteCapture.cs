using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeadAir.Tests
{
    internal static class CurrentRouteCapture
    {
        public static IEnumerator Save(string name, int width)
        {
            var camera = Camera.main;
            var target = new RenderTexture(width, width * 9 / 16, 24);
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active;
            var originals = new List<Canvas>(); var copies = new List<Canvas>();
            var png = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            try
            {
                target.Create(); camera.targetTexture = target;
                foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                {
                    if (!canvas.enabled || canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                    // Snapshot UI only; never mutate the live Canvas mode or its accumulated TMP layout.
                    var copy = Object.Instantiate(canvas.gameObject).GetComponent<Canvas>();
                    copies.Add(copy); originals.Add(canvas); canvas.enabled = false;
                    copy.renderMode = RenderMode.ScreenSpaceCamera; copy.worldCamera = camera; copy.planeDistance = 1;
                    copy.sortingOrder = 30000 + copies.Count;
                }
                yield return null; yield return null;
                Canvas.ForceUpdateCanvases();
                foreach (var canvas in copies)
                foreach (var text in canvas.GetComponentsInChildren<TMP_Text>())
                { text.ForceMeshUpdate(); Assert.That(text.isTextOverflowing, Is.False, name + "/" + width + "/" + text.name); }
                Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
                RenderTexture.active = target; png.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0); png.Apply();
                Directory.CreateDirectory("TestResults/CurrentPlayable");
                File.WriteAllBytes("TestResults/CurrentPlayable/" + name + "-" + width + ".png", png.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = oldTarget; RenderTexture.active = oldActive;
                foreach (var copy in copies) { copy.gameObject.SetActive(false); Object.Destroy(copy.gameObject); }
                foreach (var canvas in originals) canvas.enabled = true;
                Object.Destroy(png); target.Release(); Object.Destroy(target);
            }
        }
    }
}
