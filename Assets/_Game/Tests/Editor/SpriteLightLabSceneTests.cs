using System.Linq;
using DeadAir.Studies;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadAir.Tests
{
    public sealed class SpriteLightLabSceneTests
    {
        [Test]
        public void SavedLabReopensWithPrivateResourcesAndNoNativeLightingOrBuildEntry()
        {
            const string scenePath="Assets/_Game/Scenes/Studies/SpriteLightLab.unity";
            for(int pass=0;pass<2;pass++)
            {
                var scene=EditorSceneManager.OpenScene(scenePath);
                foreach(var script in scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<MonoBehaviour>(true))) Assert.That(script,Is.Not.Null);
                var field=Object.FindFirstObjectByType<LabLightField>();
                Assert.That(field.warm!=null&&field.neon!=null,Is.True);
                Assert.That(field.targets.Length,Is.GreaterThan(15));
                foreach(var renderer in field.targets)
                {
                    Assert.That(renderer,Is.Not.Null);
                    Assert.That(AssetDatabase.GetAssetPath(renderer.sharedMaterial).StartsWith("Assets/_Game/Data/Studies/SpriteLightLab/"),Is.True);
                    Assert.That(ShaderUtil.ShaderHasError(renderer.sharedMaterial.shader),Is.False,renderer.name);
                }
                foreach(var mesh in Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
                    Assert.That(AssetDatabase.GetAssetPath(mesh.sharedMesh).StartsWith("Assets/_Game/Data/Studies/SpriteLightLab/"),Is.True,mesh.name);
                foreach(var sprite in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
                    Assert.That(AssetDatabase.GetAssetPath(sprite.sprite).EndsWith(".png"),Is.True,"Use imported sprite assets, not transient native Sprite.Create exports");
                Assert.That(Object.FindObjectsByType<Light>(FindObjectsSortMode.None),Is.Empty);
                Assert.That(Object.FindObjectsByType<LabContactOval>(FindObjectsSortMode.None).Length,Is.EqualTo(1));
                int contactOrder=Object.FindFirstObjectByType<LabContactOval>().GetComponent<Renderer>().sortingOrder;
                Assert.That(GameObject.Find("Architecture").GetComponentsInChildren<Renderer>().All(r=>r.sortingOrder<contactOrder),Is.True,"2D sorting must not paint opaque architecture over the contact/sprites");
                Assert.That(Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None).All(r=>r.sortingOrder>contactOrder),Is.True);
                Assert.That(Camera.main.orthographicSize,Is.EqualTo(2.05f));
                Assert.That(Camera.main.transform.eulerAngles.x,Is.EqualTo(12).Within(.001f));
                Assert.That(EditorBuildSettings.scenes.Any(item=>item.path==scenePath),Is.False);
                field.Apply();
            }
        }
    }
}
