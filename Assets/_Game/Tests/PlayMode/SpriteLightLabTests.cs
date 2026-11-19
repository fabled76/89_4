using System.Collections;
using System.IO;
using System.Text;
using System.Globalization;
using DeadAir.Studies;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace DeadAir.Tests
{
    public sealed class SpriteLightLabTests
    {
        private const string Output="TestResults/SpriteLightLab/";
        [UnityTest]
        public IEnumerator AlphaCoverageDoesNotChangeWithRuntimeLight()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/Studies/SpriteLightLab.unity",new LoadSceneParameters(LoadSceneMode.Single));
#else
            Assert.Ignore("Isolated Editor lab.");
#endif
            var body=Object.FindFirstObjectByType<PlayerController2D>().GetComponent<Rigidbody2D>();body.simulated=false;
            var hero=body.GetComponentInChildren<SpriteRenderer>();var field=Object.FindFirstObjectByType<LabLightField>();
            foreach(var renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))if(renderer!=hero)renderer.enabled=false;
            Camera.main.backgroundColor=Color.clear;
            field.ambient=1;field.warm.isOn=false;field.neon.isOn=false;
            yield return Shot("alpha-neutral",1600,field,true);
            field.ambient=.32f;field.neon.isOn=true;yield return Shot("alpha-neon",1600,field,true);
            var a=new Texture2D(2,2);var b=new Texture2D(2,2);
            try
            {
                a.LoadImage(File.ReadAllBytes(Output+"alpha-neutral-1600.png"));b.LoadImage(File.ReadAllBytes(Output+"alpha-neon-1600.png"));
                var pa=a.GetPixels32();var pb=b.GetPixels32();int transparent=0,covered=0,partial=0;
                for(int i=0;i<pa.Length;i++){if(pa[i].a!=pb[i].a)Assert.Fail("Lighting altered alpha at pixel "+i);if(pa[i].a==0)transparent++;else covered++;if(pa[i].a>0&&pa[i].a<255)partial++;}
                Assert.That(transparent,Is.GreaterThan(0));Assert.That(covered,Is.GreaterThan(5000));Assert.That(partial,Is.GreaterThan(0));
                File.WriteAllText(Output+"alpha-check.json","{\"unchanged\":true,\"coveredPixels\":"+covered+",\"partialEdgePixels\":"+partial+"}");
            }
            finally{Object.Destroy(a);Object.Destroy(b);}
            LogAssert.NoUnexpectedReceived();
        }
        [UnityTest]
        public IEnumerator RuntimeLightsDistanceSidesControlsFloorOvalAndTraversal()
        {
            var previous=InputSystem.settings;var settings=Object.Instantiate(previous);
            settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings=settings;var keyboard=InputSystem.AddDevice<Keyboard>();
            try
            {
#if UNITY_EDITOR
                yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/_Game/Scenes/Studies/SpriteLightLab.unity",new LoadSceneParameters(LoadSceneMode.Single));
#else
                Assert.Ignore("Isolated Editor lab.");
#endif
                yield return new WaitForSeconds(.25f);
                var field=Object.FindFirstObjectByType<LabLightField>();var controls=Object.FindFirstObjectByType<LabReviewControls>();
                var actor=controls.actor;var oval=Object.FindFirstObjectByType<LabContactOval>();
                var hero=actor.GetComponentInChildren<SpriteRenderer>();
                field.ambient=1;field.warm.isOn=false;field.neon.isOn=false;
                yield return Pair("01-neutral",field);
                field.ambient=.32f;yield return Pair("02-half-shadow",field);
                field.warm.isOn=true;yield return Pair("03-warm",field);
                field.warm.isOn=false;field.neon.isOn=true;yield return Pair("04-neon",field);
                field.warm.isOn=true;yield return Pair("05-both",field);
                field.neon.isOn=false;yield return Pair("06-neon-off-again",field);
                field.warm.isOn=false;field.neon.isOn=true;
                Vector3 originalNeon=field.neon.transform.position;
                field.neon.transform.position=new Vector3(actor.position.x-1.25f,1.1f,-.55f);yield return Pair("07-neon-left",field);
                field.neon.transform.position=new Vector3(actor.position.x+1.25f,1.1f,-.55f);yield return Pair("08-neon-right",field);
                field.neon.transform.position=new Vector3(8,1.1f,-.55f);yield return Pair("09-neon-far",field);
                Assert.That(LabLightSource.Falloff(0,2.3f),Is.EqualTo(1));
                Assert.That(LabLightSource.Falloff(2.3f,2.3f),Is.Zero);
                Assert.That(LabLightSource.Falloff(.8f,2.3f),Is.GreaterThan(LabLightSource.Falloff(1.6f,2.3f)));
                field.neon.transform.position=originalNeon;
                field.warm.isOn=true;field.neon.isOn=true;
                // A facing diagnostic changes only the visual; the floor anchor must not follow it.
                oval.Apply();Vector3 before=oval.transform.position;
                hero.flipX=true;yield return null;oval.Apply();Assert.That(oval.transform.position,Is.EqualTo(before));
                yield return Pair("10-facing-diagnostic",field);hero.flipX=false;
                yield return Press(keyboard,Key.Digit1);Assert.That(field.warm.isOn,Is.False);
                yield return Press(keyboard,Key.Digit2);Assert.That(field.neon.isOn,Is.False);
                yield return Press(keyboard,Key.H);Assert.That(field.ambient,Is.EqualTo(1));
                yield return Press(keyboard,Key.H);Assert.That(field.ambient,Is.EqualTo(.32f));
                yield return Press(keyboard,Key.R);Assert.That(actor.position.x,Is.EqualTo(.35f).Within(.01f));
                // Use the real existing A/D controller for both architectural bounds and stop checks.
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A));yield return new WaitForSeconds(2.7f);
                Assert.That(actor.position.x,Is.InRange(-3.02f,-2.93f));
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.1f);
                float stopped=actor.position.x;yield return new WaitForSeconds(.1f);Assert.That(actor.position.x,Is.EqualTo(stopped).Within(.001f));
                field.warm.isOn=true;field.neon.isOn=true;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));
                Directory.CreateDirectory(Output+"traversal");var csv=new StringBuilder("frame,gameTime,rootX,rootY,footY\n");
                for(int frame=0;frame<36;frame++)
                {
                    yield return new WaitForSeconds(.1f);
                    yield return Shot("traversal/frame-"+frame.ToString("D3"),960,field);
                    csv.AppendLine(string.Join(",",frame,Time.time.ToString("F6",CultureInfo.InvariantCulture),actor.position.x.ToString("F6",CultureInfo.InvariantCulture),actor.position.y.ToString("F6",CultureInfo.InvariantCulture),(actor.position.y-.9f).ToString("F6",CultureInfo.InvariantCulture)));
                    Assert.That(actor.position.y-.9f,Is.InRange(-.01f,.05f));
                    Assert.That(actor.transform.position.z,Is.EqualTo(-.15f));
                    Assert.That(oval.transform.position.y,Is.EqualTo(.006f).Within(.00001f));
                }
                yield return new WaitForSeconds(.9f);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.1f);
                Assert.That(actor.position.x,Is.InRange(2.93f,3.02f));
                File.WriteAllText(Output+"traversal/timing.csv",csv.ToString());
                var watch=System.Diagnostics.Stopwatch.StartNew();for(int sample=0;sample<250;sample++)field.Apply();watch.Stop();
                File.WriteAllText(Output+"update-cost.json","{\"targets\":"+field.targets.Length+",\"samples\":250,\"meanMilliseconds\":"+(watch.Elapsed.TotalMilliseconds/250).ToString("R",CultureInfo.InvariantCulture)+",\"scope\":\"CPU property-block updates only, not GPU/frame-rate profiling\"}");
                LogAssert.NoUnexpectedReceived();
            }
            finally{InputSystem.RemoveDevice(keyboard);InputSystem.settings=previous;Object.Destroy(settings);}
        }

        private static IEnumerator Press(Keyboard keyboard,Key key)
        {InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;}
        private static IEnumerator Pair(string name,LabLightField field)
        {foreach(int width in new[]{1600,960})yield return Shot(name,width,field);}
        private static IEnumerator Shot(string name,int width,LabLightField field,bool alpha=false)
        {
            yield return null;yield return null;field.Apply();
            var camera=Camera.main;var target=new RenderTexture(width,width*9/16,24);
            var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;float aspect=camera.aspect;
            var png=new Texture2D(width,target.height,alpha?TextureFormat.RGBA32:TextureFormat.RGB24,false);
            try
            {
                target.Create();camera.aspect=16f/9;camera.targetTexture=target;
                RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});
                RenderTexture.active=target;png.ReadPixels(new Rect(0,0,width,target.height),0,0);png.Apply();
                string path=Output+name+"-"+width+".png";Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllBytes(path,png.EncodeToPNG());
            }
            finally{camera.targetTexture=oldTarget;camera.aspect=aspect;RenderTexture.active=oldActive;Object.Destroy(png);target.Release();Object.Destroy(target);}
        }
    }
}
