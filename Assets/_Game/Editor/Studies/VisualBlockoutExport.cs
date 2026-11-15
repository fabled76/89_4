using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DeadAir.Editor.Studies
{
    public static class VisualBlockoutExport
    {
        public const string Output = "output/art/visual_direction_comparison/blockout_v01/";
        [Serializable] private sealed class CameraRecord
        {
            public float[] position, eulerDegrees, quaternionXYZW, forward, up, right, target;
            public float orthographicHalfHeight, projectedWidth, projectedHeight, nearClip, farClip;
            public int[] resolution;
            public float[] worldToCameraRowMajor, cameraToWorldRowMajor, projectionRowMajor;
        }
        [Serializable] private sealed class ObjectRecord
        {
            public string name, hierarchy;
            public float[] boundsMin, boundsMax, dimensionsXYZ, worldPosition, localScale, projectedRectTopLeftPixels;
            public float[] localToWorldRowMajor;
            public string[] meshParts;
        }
        [Serializable] private sealed class Anchor
        {
            public string name; public float[] unityWorld, pixelTopLeft;
        }
        [Serializable] private sealed class Manifest
        {
            public string status = "Provisional measured comparison, not final art/camera/PPU standard";
            public string sourceScene = VisualBlockoutBuilder.ScenePath;
            public string unityVersion, proBuilderVersion, rendererAsset, colorSpace;
            public string packageReference = "https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe/com-unity-probuilder";
            public string axes = "Unity left-handed: +X right, +Y up, +Z toward rear wall; room floor Y=0; units metres.";
            public string objAxes = "OBJ right-handed, Y-up: (X,Y,Z)obj=(X,Y,-Z)unity; transforms baked; reversed triangle winding. 1 numeric unit = 1 metre.";
            public string objExporter = "Study utility exports actual ProBuilder MeshFilters as triangulated Wavefront OBJ/MTL; no camera/scripts/colliders transfer.";
            public float[] unityToObjRowMajor = { 1,0,0,0, 0,1,0,0, 0,0,-1,0, 0,0,0,1 };
            public float[] nominalRoomWidthDepthHeight = { 6.4f,3,2.6f };
            public float designGrid = .10f, assemblySnap = .05f, fixedPathZ = VisualBlockoutBuilder.PathZ;
            public CameraRecord camera;
            public ObjectRecord[] objects, meshObjects;
            public Anchor[] anchors;
            public float wallSeamYpx, feetYpx, roomFrontOpeningYpx, viewportForegroundYpx;
            public float rejectedPitch = 8, rejectedBackFloorPixels, selectedBackFloorPixels, selectedFrontRoomPixels;
            public string projectionBoundsMeaning = "Unoccluded mesh-vertex projection envelopes, not visible segmentation masks; top-left x,y,width,height.";
            public string[] exceptions, reachNotes;
            public string[] standingEvidence = { "standing_door_1920.png", "standing_desk_1920.png", "standing_bed_1920.png" };
        }
        [MenuItem("89.4/Studies/Export Measured Blockout Handoff")]
        public static void FromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            BuildAndExport();
        }
        public static void BuildAndExport()
        {
            if (!File.Exists(VisualBlockoutBuilder.ScenePath)) throw new FileNotFoundException("Open/build the study explicitly before exporting.", VisualBlockoutBuilder.ScenePath);
            EditorSceneManager.OpenScene(VisualBlockoutBuilder.ScenePath);
            Directory.CreateDirectory(Output); Directory.CreateDirectory(Output + "diagnostics");
            var camera = Camera.main; var person = GameObject.Find("ReferencePerson").transform;
            Vector3 original = person.position, cameraPosition = camera.transform.position;
            Quaternion cameraRotation = camera.transform.rotation;
            float originalAspect = camera.aspect;
            // Editor Game-view aspect is not serialized on Camera. Projection and guide must
            // use the same explicit 16:9 aspect as the exported RenderTextures.
            camera.aspect = 16f / 9f;
            try
            {
                VisualBlockoutBuilder.SetCamera(camera, 8);
                Render(camera, Output + "diagnostics/camera_candidate_08_1920.png", 1920, 1080);
                camera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
                Render(camera, Output + "blockout_base_1920.png", 1920, 1080);
                Render(camera, Output + "blockout_base_1280.png", 1280, 720);
                Render(camera, Output + "blockout_base_960.png", 960, 540);
                Guide(camera);
                foreach (var pose in new[] { ("door", -2.25f), ("desk", -.4f), ("bed", 1.3f) })
                {
                    person.position = new Vector3(pose.Item2, .9f, VisualBlockoutBuilder.PathZ);
                    Render(camera, Output + "standing_" + pose.Item1 + "_1920.png", 1920, 1080);
                    Render(camera, Output + "standing_" + pose.Item1 + "_960.png", 960, 540);
                }
                person.position = original;
                WriteObj(GameObject.Find("MeasuredGeometry").transform);
                WriteManifest(camera);
                Debug.Log("Real Unity neutral render, separate guide, standing evidence, measured OBJ/MTL and camera/layout manifest exported to " + Output);
            }
            finally
            {
                person.position = original; camera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
                camera.aspect = originalAspect;
                // Export does not save pose/camera experiments into any scene.
            }
        }
        // One-time presentation repair: keep every mesh, transform and camera untouched.
        public static void RepairNeutralContrastAndExport()
        {
            EditorSceneManager.OpenScene(VisualBlockoutBuilder.ScenePath);
            string directory = VisualBlockoutBuilder.Data + "Materials/";
            var values = new Dictionary<string, float> {
                { "Room", .62f }, { "Door", .42f }, { "Desk", .45f }, { "Chair", .38f },
                { "Bed", .58f }, { "TravelBag", .32f }, { "Receiver", .20f }, { "ReferencePerson", .27f }, { "Calibration_OutsideFrame", .55f }
            };
            foreach (Transform group in GameObject.Find("MeasuredGeometry").transform)
            {
                if (!values.TryGetValue(group.name, out float value)) throw new InvalidOperationException("Undeclared neutral group " + group.name);
                var palette = new Material[5];
                float[] offsets = { .12f, 0, -.08f, -.14f, -.20f };
                for (int i = 0; i < palette.Length; i++)
                {
                    string path = directory + group.name + "_" + i + ".mat";
                    var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Unlit")); AssetDatabase.CreateAsset(material, path); }
                    material.name = group.name + "_" + i;
                    material.SetColor("_BaseColor", new Color(value + offsets[i], value + offsets[i], value + offsets[i]));
                    EditorUtility.SetDirty(material); palette[i] = material;
                }
                foreach (var renderer in group.GetComponentsInChildren<MeshRenderer>()) renderer.sharedMaterials = palette;
            }
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            BuildAndExport();
        }
        public static void Render(Camera camera, string path, int width, int height)
        {
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float aspect = camera.aspect;
            var png = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                target.Create(); camera.aspect = width / (float)height; camera.targetTexture = target;
                Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = target });
                RenderTexture.active = target; png.ReadPixels(new Rect(0,0,width,height),0,0); png.Apply();
                Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllBytes(path, png.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = oldTarget; camera.aspect = aspect; RenderTexture.active = oldActive;
                Object.DestroyImmediate(png); target.Release(); Object.DestroyImmediate(target);
            }
        }
        private static void Guide(Camera camera)
        {
            var root = new GameObject("TemporaryGuide_NotRuntime", typeof(Canvas), typeof(CanvasScaler));
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera;
            canvas.planeDistance = .25f; canvas.sortingOrder = 32000;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920,1080);
            try
            {
                Label("MEASURED BLOCKOUT v01 / comparison only / one unit = one metre", new Vector2(25,20), 1050);
                Label("Orthographic 18 deg down / 8.0 x 4.5 projected m / no final camera or style approval", new Vector2(25,57), 1150);
                var seamA = Pixel(camera, new Vector3(-3.2f,0,1.5f)); var seamB = Pixel(camera, new Vector3(3.2f,0,1.5f));
                var footA = Pixel(camera, new Vector3(-3.2f,0,VisualBlockoutBuilder.PathZ)); var footB = Pixel(camera, new Vector3(3.2f,0,VisualBlockoutBuilder.PathZ));
                Line(seamA, seamB, Color.cyan); Label("CYAN: rear wall / floor junction", new Vector2(25,seamA.y-36), 430);
                Line(footA, footB, Color.magenta); Label("MAGENTA: fixed horizontal foot path", new Vector2(25,footA.y+8), 490);
                Line(new Vector2(0,1077), new Vector2(1920,1077), Color.yellow);
                Label("YELLOW: viewport foreground; coplanar presentation apron continues beyond room opening", new Vector2(25,1037), 1130);
                var bottom = Pixel(camera,new Vector3(1.38f,0,VisualBlockoutBuilder.PathZ));
                var top = Pixel(camera,new Vector3(1.38f,1.8f,VisualBlockoutBuilder.PathZ));
                Line(bottom,top,Color.green); Label("1.80 m WORLD height", new Vector2(top.x+10,top.y), 300);
                AnchorLabel("Door leaf .85 x 2.10 / handle Y=1.00", new Vector2(180,230), new Vector3(-1.95f,1,1.435f));
                AnchorLabel("Desk 1.35 x .65 / top Y=.74", new Vector2(570,305), new Vector3(-.4f,.74f,1.1f));
                AnchorLabel("Chair seat Y=.45 / overall .85", new Vector2(470,565), new Vector3(-.4f,.45f,.62f));
                AnchorLabel("Receiver .18 x .11 x .05 / antenna separate", new Vector2(720,390), new Vector3(-.4f,.795f,.85f));
                AnchorLabel("Bag .45 x .25 / total height .30", new Vector2(250,925), new Vector3(-1.35f,.15f,.75f));
                AnchorLabel("Bed mattress .90 x 1.95 / top Y=.50", new Vector2(1280,470), new Vector3(2,.5f,1));
                Render(camera,Output+"blockout_guide_1920.png",1920,1080);
            }
            finally { Object.DestroyImmediate(root); }
            void Label(string text, Vector2 topLeft, float width)
            {
                var go = new GameObject("GuideLabel",typeof(RectTransform),typeof(Image)); go.transform.SetParent(root.transform,false);
                var rect=go.GetComponent<RectTransform>(); rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
                rect.anchoredPosition=new Vector2(topLeft.x,-topLeft.y); rect.sizeDelta=new Vector2(width,32);
                go.GetComponent<Image>().color=new Color(0,0,0,.84f);
                var child=new GameObject("Text",typeof(RectTransform),typeof(Text)); child.transform.SetParent(go.transform,false);
                var r=child.GetComponent<RectTransform>(); r.anchorMin=Vector2.zero; r.anchorMax=Vector2.one; r.offsetMin=new Vector2(5,0); r.offsetMax=Vector2.zero;
                var label=child.GetComponent<Text>(); label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.fontSize=21; label.text=text;
                label.color=Color.white; label.raycastTarget=false;
            }
            void Line(Vector2 a,Vector2 b,Color color)
            {
                var go=new GameObject("GuideLine",typeof(RectTransform),typeof(Image)); go.transform.SetParent(root.transform,false);
                var rect=go.GetComponent<RectTransform>(); rect.anchorMin=rect.anchorMax=new Vector2(0,1);
                rect.anchoredPosition=new Vector2((a.x+b.x)/2,-(a.y+b.y)/2); rect.sizeDelta=new Vector2(Vector2.Distance(a,b),3);
                rect.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(-(b.y-a.y),b.x-a.x)*Mathf.Rad2Deg);
                go.GetComponent<Image>().color=color;
            }
            void AnchorLabel(string text,Vector2 topLeft,Vector3 world)
            { Line(topLeft+new Vector2(140,32),Pixel(camera,world),new Color(.8f,.8f,.8f)); Label(text,topLeft,540); }
        }
        private static void WriteManifest(Camera camera)
        {
            var root = GameObject.Find("MeasuredGeometry").transform;
            var anchors = new[]
            {
                Point("foot",new Vector3(.9f,0,VisualBlockoutBuilder.PathZ)), Point("head_top",new Vector3(.9f,1.8f,VisualBlockoutBuilder.PathZ)),
                Point("door_handle",new Vector3(-1.95f,1,1.435f)), Point("desk_surface",new Vector3(-.4f,.74f,1.1f)),
                Point("chair_seat",new Vector3(-.4f,.45f,.62f)), Point("bed_sleeping_top",new Vector3(2,.5f,1)),
                Point("receiver_body_center",new Vector3(-.4f,.795f,.85f)), Point("bag_foot",new Vector3(-1.35f,0,.75f)),
                Point("wall_floor_seam",new Vector3(0,0,1.5f)), Point("nominal_room_front",new Vector3(0,0,-1.5f))
            };
            var record=new Manifest
            {
                unityVersion=Application.unityVersion,proBuilderVersion=UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(ProBuilderMesh).Assembly).version,
                rendererAsset=AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),colorSpace=QualitySettings.activeColorSpace.ToString(),
                camera=new CameraRecord
                {
                    position=V(camera.transform.position),eulerDegrees=V(camera.transform.eulerAngles),
                    quaternionXYZW=new[]{camera.transform.rotation.x,camera.transform.rotation.y,camera.transform.rotation.z,camera.transform.rotation.w},
                    forward=V(camera.transform.forward),up=V(camera.transform.up),right=V(camera.transform.right),target=V(VisualBlockoutBuilder.CameraTarget),
                    orthographicHalfHeight=camera.orthographicSize,projectedWidth=camera.orthographicSize*2*16/9f,projectedHeight=camera.orthographicSize*2,
                    nearClip=camera.nearClipPlane,farClip=camera.farClipPlane,resolution=new[]{1920,1080},
                    worldToCameraRowMajor=M(camera.worldToCameraMatrix),cameraToWorldRowMajor=M(camera.cameraToWorldMatrix),projectionRowMajor=M(camera.projectionMatrix)
                },
                objects=root.Cast<Transform>().Select(transform=>Describe(camera,transform)).ToArray(),
                meshObjects=root.GetComponentsInChildren<MeshFilter>().Select(filter=>Describe(camera,filter.transform)).ToArray(), anchors=anchors,
                wallSeamYpx=Pixel(camera,new Vector3(0,0,1.5f)).y,feetYpx=Pixel(camera,new Vector3(0,0,VisualBlockoutBuilder.PathZ)).y,
                roomFrontOpeningYpx=Pixel(camera,new Vector3(0,0,-1.5f)).y,viewportForegroundYpx=1080,
                rejectedBackFloorPixels=1.25f*Mathf.Sin(8*Mathf.Deg2Rad)*240,
                selectedBackFloorPixels=1.25f*Mathf.Sin(VisualBlockoutBuilder.CameraPitch*Mathf.Deg2Rad)*240,
                selectedFrontRoomPixels=1.75f*Mathf.Sin(VisualBlockoutBuilder.CameraPitch*Mathf.Deg2Rad)*240,
                exceptions=new[]{
                    "Selected 18 degrees after testing candidate 8: nominal 3m floor needs readable projected depth. Not a game camera migration.",
                    "Nominal clear room 6.4 x 3.0 x 2.6m. Cutaway floor slab 8.8 x 6.1m extends beyond the open front to avoid a platform edge. No extra navigable depth.",
                    "Bed sleeping surface is 1.95 x .90m, top .50m. Headboard adds .04m to overall X footprint and reaches .90m high.",
                    "Neutral segmented 1.80m reference, no costume/identity/animation rig. Projected tilted height is measured, never stretched to 432px.",
                    "Calibration 1m cube at X=5 is outside the image but included in OBJ. No new runtime art or final material system.",
                    "Flat gray face-value materials use existing URP Unlit and current Renderer2D. No renderer/global lighting changes; no Blender render claimed."
                },
                reachNotes=new[]{
                    "Horizontal movement reuses PlayerController2D on XY, Z fixed .25. 2D collision support at Y=0; no vertical/depth input.",
                    "Door/receiver/bag use existing InteractionController/InspectInteractable proxies. 2D proximity is not literal 3D hand contact.",
                    "Rear door handle Z=1.435 is 1.185m behind path: requires a controlled door cut in future gameplay, no invented long reach.",
                    "Receiver near table front Z=.85 is .60m behind path; a raised hand near Y=1.0 can reach within roughly .7m.",
                    "Chair front .395 and bed front .55 stay behind mannequin maximum Z=.36, leaving the horizontal lane clear."
                }
            };
            File.WriteAllText(Output+"camera_layout_manifest.json",JsonUtility.ToJson(record,true));
            Anchor Point(string name,Vector3 world)=>new Anchor{name=name,unityWorld=V(world),pixelTopLeft=new[]{Pixel(camera,world).x,Pixel(camera,world).y}};
        }
        private static ObjectRecord Describe(Camera camera,Transform root)
        {
            var filters=root.GetComponentsInChildren<MeshFilter>(); var vertices=filters.SelectMany(filter=>filter.sharedMesh.vertices.Select(vertex=>filter.transform.TransformPoint(vertex))).ToArray();
            var bounds=new Bounds(vertices[0],Vector3.zero); foreach(var vertex in vertices)bounds.Encapsulate(vertex);
            var pixels=vertices.Select(vertex=>Pixel(camera,vertex)).ToArray(); float minX=pixels.Min(p=>p.x),minY=pixels.Min(p=>p.y);
            string hierarchy=root.name; for(var parent=root.parent;parent!=null;parent=parent.parent) hierarchy=parent.name+"/"+hierarchy;
            return new ObjectRecord{name=root.name,hierarchy=hierarchy,boundsMin=V(bounds.min),boundsMax=V(bounds.max),dimensionsXYZ=V(bounds.size),
                worldPosition=V(root.position),localScale=V(root.localScale),localToWorldRowMajor=M(root.localToWorldMatrix),
                projectedRectTopLeftPixels=new[]{minX,minY,pixels.Max(p=>p.x)-minX,pixels.Max(p=>p.y)-minY},meshParts=filters.Select(filter=>filter.name).ToArray()};
        }
        private static void WriteObj(Transform root)
        {
            var obj=new StringBuilder("# VisualBlockout v01; metres; world transforms baked; right-handed Y-up; unity Z mirrored\nmtllib blockout_geometry.mtl\n");
            var mtl=new StringBuilder("# Neutral comparison materials, not a final art palette\n"); var materials=new Dictionary<Material,string>(); int offset=0;
            foreach(var filter in root.GetComponentsInChildren<MeshFilter>().OrderBy(filter=>filter.transform.parent.name+"/"+filter.name,StringComparer.Ordinal))
            {
                var mesh=filter.sharedMesh; var matrix=filter.transform.localToWorldMatrix; var normals=mesh.normals; var uvs=mesh.uv;
                obj.AppendLine("o "+filter.transform.parent.name+"__"+filter.name);
                foreach(var vertex in mesh.vertices){var v=matrix.MultiplyPoint3x4(vertex);obj.AppendLine("v "+F(v.x)+" "+F(v.y)+" "+F(-v.z));}
                for(int i=0;i<mesh.vertexCount;i++){var uv=i<uvs.Length?uvs[i]:Vector2.zero;obj.AppendLine("vt "+F(uv.x)+" "+F(uv.y));}
                for(int i=0;i<mesh.vertexCount;i++){var n=matrix.inverse.transpose.MultiplyVector(normals[i]).normalized;obj.AppendLine("vn "+F(n.x)+" "+F(n.y)+" "+F(-n.z));}
                var slots=filter.GetComponent<MeshRenderer>().sharedMaterials;
                for(int sub=0;sub<mesh.subMeshCount;sub++)
                {
                    var material=slots[Mathf.Min(sub,slots.Length-1)];
                    if(!materials.TryGetValue(material,out string name))
                    {
                        name=material.name;materials.Add(material,name);Color color=material.GetColor("_BaseColor");
                        mtl.AppendLine("newmtl "+name+"\nKd "+F(color.r)+" "+F(color.g)+" "+F(color.b)+"\nKa 0 0 0\nd 1\nillum 1\n");
                    }
                    obj.AppendLine("usemtl "+name);var triangles=mesh.GetTriangles(sub);
                    for(int i=0;i<triangles.Length;i+=3)
                    {
                        int a=offset+triangles[i]+1,b=offset+triangles[i+2]+1,c=offset+triangles[i+1]+1;
                        obj.AppendLine($"f {a}/{a}/{a} {b}/{b}/{b} {c}/{c}/{c}");
                    }
                }
                offset+=mesh.vertexCount;
            }
            File.WriteAllText(Output+"blockout_geometry.obj",obj.ToString()); File.WriteAllText(Output+"blockout_geometry.mtl",mtl.ToString());
        }
        private static Vector2 Pixel(Camera camera,Vector3 world)
        {var p=camera.WorldToViewportPoint(world);return new Vector2(p.x*1920,(1-p.y)*1080);}
        private static float[] V(Vector3 value)=>new[]{value.x,value.y,value.z};
        private static float[] M(Matrix4x4 matrix)=>Enumerable.Range(0,16).Select(i=>matrix[i/4,i%4]).ToArray();
        private static string F(float value)=>value.ToString("G9",CultureInfo.InvariantCulture);
    }
}
