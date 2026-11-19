using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

namespace DeadAir.Editor.Studies
{
    // Explicit construction only. Opening or playing the saved scene never regenerates it.
    public static class SpriteRoomBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/Studies/SpriteRoom_ProBuilder.unity";
        const string Data = "Assets/_Game/Data/Studies/SpriteRoom/";
        const string Art = "Assets/_Game/Art/Studies/SpriteRoom/Textures/";
        const string Output = "output/sprite_room_probuilder/";
        static Material coral, reveal, wood, trim, burgundy, teal, desk;
        static readonly Quaternion Pitch = Quaternion.Euler(12, 0, 0);

        [MenuItem("89.4/Studies/Sprite Room/Open Saved Scene")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
        }
        [MenuItem("89.4/Studies/Sprite Room/Create Once (Never Overwrites)")]
        public static void Create()
        {
            if (File.Exists(ScenePath)) { Open(); return; }
            Directory.CreateDirectory(Data + "Meshes"); Directory.CreateDirectory(Data + "Materials");
            Directory.CreateDirectory(Output); AssetDatabase.Refresh();
            Import("Hero_Selected_Revision02.png", true); Import("Exterior_Temporary_NotCanon.png", false);
            Import("Wood_CC0.jpg", false); Import("Stucco_CC0.jpg", false);
            coral = Mat("Coral_Stucco_Unlit", "#CE9589", "Stucco_CC0.jpg");
            reveal = Mat("Coral_Reveals_Unlit", "#AE746D", "Stucco_CC0.jpg");
            wood = Mat("Wood_Floor_Unlit", "#D7BAA5", "Wood_CC0.jpg");
            trim = Mat("Ivory_Trim_Unlit", "#CFC1AC");
            burgundy = Mat("Placeholder_Burgundy_Unlit", "#7C4258");
            teal = Mat("Placeholder_Teal_Unlit", "#507F80");
            desk = Mat("Placeholder_Walnut_Unlit", "#806353");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var architecture = Group("Architecture");
            var rear = Group("RearWall_Thickness_0.18m", architecture);
            Box("Floor_6.4x3m_TopY0", architecture, new Vector3(0,-.10f,0), new Vector3(8.8f,.20f,3.2f), wood, 2);
            Box("Floor_FrontApron_NoGameplay", architecture, new Vector3(0,-.10f,-3.7f), new Vector3(8.8f,.20f,4.2f), wood, 2);
            // Door clear opening x[-2.775,-1.825], y[0,2.2]; window x[-.6,1], y[.9,2.1].
            Wall("Door_LeftPier", rear,-3.2f,-2.775f,0,2.7f);
            Wall("Door_Header", rear,-2.775f,-1.825f,2.2f,2.7f);
            Wall("Between_Door_And_Window",rear,-1.825f,-.6f,0,2.7f);
            Wall("Window_SillWall",rear,-.6f,1,0,.9f);
            Wall("Window_Header",rear,-.6f,1,2.1f,2.7f);
            Wall("Window_RightPier",rear,1,3.2f,0,2.7f);
            Box("Return_Left_0.65m",architecture,new Vector3(-3.29f,1.35f,1.185f),new Vector3(.18f,2.7f,.65f),reveal,2);
            Box("Return_Right_0.65m",architecture,new Vector3(3.29f,1.35f,1.185f),new Vector3(.18f,2.7f,.65f),reveal,2);
            var moulding=Group("Restrained_Architectural_Trim",architecture);
            Box("Skirting_Left",moulding,new Vector3(-2.9875f,.045f,1.485f),new Vector3(.425f,.09f,.03f),trim,1);
            Box("Skirting_Main",moulding,new Vector3(.6875f,.045f,1.485f),new Vector3(5.025f,.09f,.03f),trim,1);
            Box("Window_Sill_RealDepth",moulding,new Vector3(.2f,.91f,1.53f),new Vector3(1.72f,.04f,.32f),trim,1);
            Box("Door_Trim_L",moulding,new Vector3(-2.80f,1.115f,1.48f),new Vector3(.05f,2.23f,.04f),trim,1);
            Box("Door_Trim_R",moulding,new Vector3(-1.80f,1.115f,1.48f),new Vector3(.05f,2.23f,.04f),trim,1);
            Box("Door_Trim_Top",moulding,new Vector3(-2.3f,2.225f,1.48f),new Vector3(1.05f,.05f,.04f),trim,1);

            var slots=Group("SpriteSlots");
            Slot("Desk_PLACEHOLDER_1.35x0.74",slots,new Vector3(-.85f,0,.55f),new Vector2(1.35f,.74f),desk,"DESK / placeholder");
            Slot("Chair_PLACEHOLDER_0.45x0.85",slots,new Vector3(-.9f,0,.15f),new Vector2(.45f,.85f),burgundy,"CHAIR");
            Slot("Bed_PLACEHOLDER_1.95x0.90",slots,new Vector3(2.05f,0,.65f),new Vector2(1.95f,.90f),burgundy,"BED / placeholder");
            Slot("Receiver_PLACEHOLDER_0.18x0.11",slots,new Vector3(-.55f,.74f,.48f),new Vector2(.18f,.11f),teal,"");
            var window=Group("WindowFrame_SPRITE_SLOT_Placeholder",slots);
            // Separate flat strips, no hidden solid wall across the opening.
            Flat("Frame_L_PLACEHOLDER",window,new Vector3(-.59f,1.5f,1.475f),new Vector2(.045f,1.20f),teal);
            Flat("Frame_R_PLACEHOLDER",window,new Vector3(.99f,1.5f,1.475f),new Vector2(.045f,1.20f),teal);
            Flat("Frame_Top_PLACEHOLDER",window,new Vector3(.2f,2.08f,1.475f),new Vector2(1.60f,.045f),teal);
            Flat("Frame_Bottom_PLACEHOLDER",window,new Vector3(.2f,.93f,1.475f),new Vector2(1.60f,.045f),teal);
            Flat("Frame_Mullion_PLACEHOLDER",window,new Vector3(.2f,1.5f,1.47f),new Vector2(.035f,1.15f),teal);
            var curtains=Group("Curtains_SPRITE_SLOT_Placeholder",slots);
            Flat("Curtain_L_PLACEHOLDER",curtains,new Vector3(-.70f,1.51f,1.40f),new Vector2(.25f,1.37f),burgundy);
            Flat("Curtain_R_PLACEHOLDER",curtains,new Vector3(1.10f,1.51f,1.40f),new Vector2(.25f,1.37f),burgundy);
            var exterior=Mat("Exterior_Preview_Unlit_NOT_CANON","#B3B1CA","Exterior_Temporary_NotCanon.png");
            Flat("OutsideView_TEMP_GasStation_NOT_CANON_NoParallax",slots,new Vector3(.2f,1.50f,2.2f),new Vector2(2.65f,1.49f),exterior);
            var doorBackdrop=Mat("Doorway_Outside_Indigo_Unlit","#343953");
            Flat("Doorway_Exterior_Preview_Plane",slots,new Vector3(-2.3f,1.1f,2.3f),new Vector2(1.18f,2.45f),doorBackdrop);
            var hero=Group("Character_Selected_Static_1.80m_FootAnchor",slots);
            hero.position=new Vector3(.85f,0,-.15f);
            var sr=hero.gameObject.AddComponent<SpriteRenderer>();
            sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"Hero_Selected_Revision02.png");
            sr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");
            if(sr.sharedMaterial==null) throw new InvalidOperationException("Package sprite unlit material not found");
            hero.rotation=Pitch;
            // Visible image already depicts the 12-degree view. Camera-aligned once, with one common scale.
            hero.localScale=Vector3.one*(1.8f*Mathf.Cos(12*Mathf.Deg2Rad)/(582.84f/331.03447f));
            var shadowMat=Mat("Contact_Shadow_Separate_Unlit","#2E2938");
            var shadow=Flat("Character_ContactShadow_PLACEHOLDER_Flat_NoCastShadow",slots,new Vector3(.85f,.004f,-.13f),new Vector2(.43f,.18f),shadowMat);
            shadow.rotation=Quaternion.Euler(90,0,0);
            // Shadow is a disclosed flat ellipse, never a hidden character model.
            var mf=shadow.GetComponent<MeshFilter>();
            var sm=Ellipse(); AssetDatabase.CreateAsset(sm,Data+"Meshes/ContactEllipse.asset"); mf.sharedMesh=sm;

            var lighting=Group("Lighting");
            var lamp=Group("Lamp_Warm_SourceAnchor_ONLY_NoDynamicLighting",lighting); lamp.position=new Vector3(-1.45f,1.8f,.8f);
            var ll=lamp.gameObject.AddComponent<Light>(); ll.type=LightType.Point; ll.color=new Color(1,.77f,.47f);ll.intensity=1;ll.range=3;ll.enabled=false;
            var outside=Group("Exterior_Cool_SourceAnchor_ONLY_NoDynamicLighting",lighting);outside.position=new Vector3(.2f,1.5f,2.5f);
            var ol=outside.gameObject.AddComponent<Light>();ol.type=LightType.Point;ol.color=new Color(.45f,.65f,1);ol.range=4;ol.enabled=false;
            var cameras=Group("Camera");
            var cam=Group("FixedCamera_12deg_Orthographic",cameras).gameObject.AddComponent<Camera>();cam.tag="MainCamera";
            ConfigureCamera(cam);
            var guides=Group("ReviewGuides");
            var path=Group("FootPath_Y0_Zminus0.15_NoDepthNavigation",guides);path.position=new Vector3(0,.01f,-.15f);
            var line=path.gameObject.AddComponent<LineRenderer>();line.positionCount=2;line.SetPositions(new[]{new Vector3(-3.1f,.01f,-.15f),new Vector3(3.1f,.01f,-.15f)});line.startWidth=line.endWidth=.015f;line.sharedMaterial=teal;
            guides.gameObject.SetActive(false);
            AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene,ScenePath);
            // Verify the serialized scene, not the builder's live object graph.
            EditorSceneManager.OpenScene(ScenePath); Validate(); RestoreFraming(); Capture();
            Debug.Log("SPRITE_ROOM_READY: saved, reopened, checked and captured. No startup/build/renderer changes.");
        }
        static void ConfigureCamera(Camera c)
        {
            c.transform.rotation=Pitch;c.transform.position=new Vector3(0,1.25f,0)-c.transform.forward*10;
            c.orthographic=true;c.orthographicSize=2.05f;c.aspect=16f/9;c.nearClipPlane=.1f;c.farClipPlane=30;
            c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(.20f,.19f,.25f);c.allowHDR=false;c.allowMSAA=false;
        }
        [MenuItem("89.4/Studies/Sprite Room/Restore Camera and Scene Framing")]
        public static void RestoreFraming()
        {
            if(EditorSceneManager.GetActiveScene().path!=ScenePath)return;
            ConfigureCamera(Camera.main);
            var sv=SceneView.lastActiveSceneView??EditorWindow.GetWindow<SceneView>();
            sv.in2DMode=false;sv.sceneLighting=false;sv.drawGizmos=false;
            sv.LookAtDirect(new Vector3(0,1.1f,.3f),Quaternion.Euler(24,-24,0),4.8f);sv.orthographic=false;sv.Repaint();
        }
        [MenuItem("89.4/Studies/Sprite Room/Capture and Verify Saved Scene")]
        public static void Capture()
        {
            if(EditorSceneManager.GetActiveScene().path!=ScenePath)throw new InvalidOperationException("Open SpriteRoom first");
            Directory.CreateDirectory(Output);Validate();
            Render(Camera.main,Output+"game_with_slots.png");
            var slots=GameObject.Find("SpriteSlots");slots.SetActive(false);
            try{Render(Camera.main,Output+"game_empty_shell.png");}finally{slots.SetActive(true);}
            var sv=SceneView.lastActiveSceneView;
            if(sv!=null)Render(sv.camera,Output+"scene_oblique.png");
            Debug.Log("SpriteRoom captures saved: "+Path.GetFullPath(Output));
        }
        [MenuItem("89.4/Studies/Sprite Room/Verify Current Scene")]
        public static void Validate()
        {
            var scene=EditorSceneManager.GetActiveScene();if(scene.path!=ScenePath)throw new InvalidOperationException("Wrong scene");
            var roots=scene.GetRootGameObjects();var meshes=roots.SelectMany(g=>g.GetComponentsInChildren<MeshFilter>(true)).ToArray();
            foreach(var m in meshes)if(m.sharedMesh==null)throw new InvalidOperationException("Missing mesh: "+m.name);
            foreach(var r in roots.SelectMany(g=>g.GetComponentsInChildren<Renderer>(true)))
                if(r.sharedMaterials.Any(m=>m==null||m.shader==null||!m.shader.isSupported))throw new InvalidOperationException("Missing/unsupported material: "+r.name);
            foreach(var g in roots.SelectMany(g=>g.GetComponentsInChildren<Transform>(true)))
                if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(g.gameObject)>0)throw new InvalidOperationException("Missing script: "+g.name);
            var pb=roots.SelectMany(g=>g.GetComponentsInChildren<ProBuilderMesh>(true)).ToArray();
            if(pb.Length<12)throw new InvalidOperationException("Expected editable ProBuilder architecture");
            // Query actual architectural collider geometry through both clear openings and the solid wall.
            Physics.SyncTransforms();
            if(Physics.Raycast(new Vector3(.0f,1.5f,1),Vector3.forward,.85f)||Physics.Raycast(new Vector3(-2.3f,1,1),Vector3.forward,.85f))throw new InvalidOperationException("An opening is blocked");
            if(!Physics.Raycast(new Vector3(2,1.5f,1),Vector3.forward,.85f))throw new InvalidOperationException("Wall geometry missing");
            File.WriteAllText(Output+"verification.txt","Saved scene reopened: "+scene.path+"\nProBuilder meshes: "+pb.Length+"\nMesh filters: "+meshes.Length+"\nMissing meshes/materials/scripts: 0\nDoor/window collider raycasts: clear; solid right pier hit\nCharacter source: revision_02 selected, static, separate contact placeholder\nPipeline/settings/build scenes: untouched by this helper\nLighting: all presentation materials UNLIT; disabled point-source anchors only\n");
        }
        static void Render(Camera cam,string path)
        {
            var prior=cam.targetTexture;var active=RenderTexture.active;var rt=RenderTexture.GetTemporary(1600,900,24,RenderTextureFormat.ARGB32);
            try{cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;var t=new Texture2D(1600,900,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1600,900),0,0);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);}
            finally{cam.targetTexture=prior;RenderTexture.active=active;RenderTexture.ReleaseTemporary(rt);}
        }
        static Transform Group(string name,Transform parent=null){var g=new GameObject(name);if(parent!=null)g.transform.SetParent(parent,false);return g.transform;}
        static void Import(string name,bool sprite)
        {
            var i=(TextureImporter)AssetImporter.GetAtPath(Art+name);i.textureType=sprite?TextureImporterType.Sprite:TextureImporterType.Default;
            i.sRGBTexture=true;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=2048;i.mipmapEnabled=!sprite;i.wrapMode=name.EndsWith("jpg")?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
            if(sprite){i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=331.03447f;var s=new TextureImporterSettings();i.ReadTextureSettings(s);s.spriteAlignment=(int)SpriteAlignment.Custom;s.spritePivot=new Vector2(.5f,30f/640);s.spriteMeshType=SpriteMeshType.FullRect;i.SetTextureSettings(s);}i.SaveAndReimport();
        }
        static Material Mat(string name,string hex,string texture=null)
        {
            var m=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name=name};ColorUtility.TryParseHtmlString(hex,out var c);m.SetColor("_BaseColor",c);m.SetFloat("_Cull",0);
            if(texture!=null)m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Art+texture));AssetDatabase.CreateAsset(m,Data+"Materials/"+name+".mat");return m;
        }
        static void Wall(string n,Transform p,float x0,float x1,float y0,float y1)=>Box(n,p,new Vector3((x0+x1)/2,(y0+y1)/2,1.6f),new Vector3(x1-x0,y1-y0,.18f),coral,2);
        static void Box(string n,Transform p,Vector3 center,Vector3 size,Material mat,float tileMetres)
        {
            var pb=ShapeGenerator.GenerateCube(PivotLocation.Center,size);pb.name=n;pb.transform.SetParent(p);pb.transform.position=center;
            var uv=pb.textures.ToArray();
            foreach(var f in pb.faces){f.manualUV=true;var idx=f.indexes;var norm=Vector3.Cross(pb.positions[idx[1]]-pb.positions[idx[0]],pb.positions[idx[2]]-pb.positions[idx[0]]).normalized;
                foreach(int j in f.distinctIndexes){var v=pb.positions[j]+center;uv[j]=(Mathf.Abs(norm.y)>.5f?new Vector2(v.x,v.z):Mathf.Abs(norm.x)>.5f?new Vector2(v.z,v.y):new Vector2(v.x,v.y))/tileMetres;}}
            pb.textures=uv;pb.GetComponent<MeshRenderer>().sharedMaterial=mat;pb.ToMesh();pb.Refresh();pb.preserveMeshAssetOnDestroy=true;
            var mesh=pb.GetComponent<MeshFilter>().sharedMesh;mesh.hideFlags=HideFlags.None;var path=Data+"Meshes/"+n+".asset";AssetDatabase.CreateAsset(mesh,path);
            var so=new SerializedObject(pb);so.FindProperty("assetGuid").stringValue=AssetDatabase.AssetPathToGUID(path);so.ApplyModifiedPropertiesWithoutUndo();
            if(pb.GetComponent<Collider>()==null)pb.gameObject.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        static Transform Flat(string n,Transform parent,Vector3 center,Vector2 size,Material m)
        {
            var g=Group(n,parent);g.position=center;var mesh=new Mesh{name=n};mesh.vertices=new[]{new Vector3(-size.x/2,-size.y/2,0),new Vector3(-size.x/2,size.y/2,0),new Vector3(size.x/2,size.y/2,0),new Vector3(size.x/2,-size.y/2,0)};
            mesh.uv=new[]{new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),new Vector2(1,0)};mesh.triangles=new[]{0,1,2,0,2,3};mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Data+"Meshes/"+n+".asset");g.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;g.gameObject.AddComponent<MeshRenderer>().sharedMaterial=m;return g;
        }
        static void Slot(string name,Transform parent,Vector3 foot,Vector2 size,Material m,string label)
        {
            var anchor=Group(name+"_FootAnchor",parent);anchor.position=foot;var plane=Flat("Plane_"+name,anchor,foot+Pitch*new Vector3(0,size.y/2,0),size,m);plane.rotation=Pitch;
            if(label.Length==0)return;
            var l=Group("PlaceholderLabel",anchor);l.position=plane.position-Pitch*Vector3.forward*.008f;l.rotation=Pitch;
            var text=l.gameObject.AddComponent<TextMesh>();text.text=label;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.characterSize=.035f;if(name.StartsWith("Desk")){text.characterSize=.022f;l.position+=Pitch*new Vector3(0,.23f,0);}text.fontSize=40;text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.GetComponent<MeshRenderer>().sharedMaterial=text.font.material;text.color=new Color(.92f,.85f,.74f);
        }
        static Mesh Ellipse()
        {
            var m=new Mesh{name="FlatContactEllipse"};var v=new Vector3[25];var uv=new Vector2[25];var tr=new int[72];
            for(int i=0;i<24;i++){float a=i*Mathf.PI*2/24;v[i+1]=new Vector3(Mathf.Cos(a)*.215f,Mathf.Sin(a)*.09f,0);tr[i*3]=0;tr[i*3+1]=i+1;tr[i*3+2]=(i+1)%24+1;}
            m.vertices=v;m.uv=uv;m.triangles=tr;m.RecalculateNormals();m.RecalculateBounds();return m;
        }
    }
}
