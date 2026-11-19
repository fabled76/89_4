using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeadAir.Editor.Studies
{
    public static class SpriteRoomFurnitureBatch01
    {
        const string Out="output/sprite_room_probuilder/";
        const string Art="Assets/_Game/Art/Studies/SpriteRoom/Textures/";
        const string Data="Assets/_Game/Data/Studies/SpriteRoom/";
        static readonly string[] Names={"Desk_PLACEHOLDER_1.35x0.74_FootAnchor","Chair_PLACEHOLDER_0.45x0.85_FootAnchor","Bed_PLACEHOLDER_1.95x0.90_FootAnchor"};
        static readonly string[] Ids={"desk","chair","bed"};
        static readonly Vector2[] Footprints={new Vector2(1.35f,.65f),new Vector2(.45f,.45f),new Vector2(1.95f,.90f)};
        static readonly float[] Heights={.74f,.85f,.85f};
        public const float Density=1080f/4.10f;

        [MenuItem("89.4/Studies/Sprite Room/Furniture Batch01 Prepare")]
        public static void Prepare()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play first");
            if(EditorSceneManager.GetActiveScene().isDirty)throw new InvalidOperationException("Unsaved user scene; preparation stopped without closing it");
            var scene=EditorSceneManager.OpenScene(SpriteRoomBuilder.ScenePath);
            var c=Camera.main;
            if(Mathf.Abs(c.orthographicSize-2.05f)>.0001f||Quaternion.Angle(c.transform.rotation,Quaternion.Euler(12,0,0))>.001f)throw new InvalidOperationException("Camera differs from frozen contract");
            Directory.CreateDirectory(Out);if(!File.Exists(Out+"furniture_batch01_material_before.png"))Render(c,Out+"furniture_batch01_material_before.png",1600,900);
            for(int i=0;i<3;i++)
            {
                var root=GameObject.Find(Names[i]).transform;
                if(root.Find("Visual")!=null)continue;
                var legacy=new GameObject("PlaceholderReference").transform;legacy.SetParent(root,false);
                foreach(var child in root.Cast<Transform>().Where(t=>t!=legacy).ToArray())child.SetParent(legacy,true);
                var group=root.gameObject.AddComponent<SortingGroup>();group.sortingOrder=-10;
                AddSprite(root,"Visual",i==1?20:10,Vector3.zero);
                AddSprite(root,"ContactShadow",i==1?19:9,-c.transform.forward*.003f);
            }
            foreach(var name in Names){var r=GameObject.Find(name).transform;r.Find("Visual").localPosition=-c.transform.forward*.50f;r.Find("ContactShadow").localPosition=-c.transform.forward*.51f;}
            Quiet("Wood_CC0.jpg","Batch01_Wood_Quiet.png",new Color(.52f,.40f,.30f),.32f);
            Quiet("Stucco_CC0.jpg","Batch01_Stucco_Quiet.png",new Color(.85f,.84f,.81f),.24f);
            SetMap("Wood_Floor_Unlit","Batch01_Wood_Quiet.png");
            SetMap("Coral_Stucco_Unlit","Batch01_Stucco_Quiet.png");
            SetMap("Coral_Reveals_Unlit","Batch01_Stucco_Quiet.png");
            var guides=scene.GetRootGameObjects().Single(g=>g.name=="ReviewGuides");
            var batch=guides.transform.Find("FurnitureBatch01_Targets");
            if(batch==null)
            {
                batch=new GameObject("FurnitureBatch01_Targets").transform;batch.SetParent(guides.transform,false);
                var mat=AssetDatabase.LoadAssetAtPath<Material>(Data+"Materials/Placeholder_Teal_Unlit.mat");
                for(int i=0;i<3;i++)
                {
                    var p=GameObject.Find(Names[i]).transform.position;float w=Footprints[i].x/2,d=Footprints[i].y/2;
                    Line(batch,Ids[i]+"_Footprint",new[]{p+new Vector3(-w,.012f,-d),p+new Vector3(-w,.012f,d),p+new Vector3(w,.012f,d),p+new Vector3(w,.012f,-d),p+new Vector3(-w,.012f,-d)},mat);
                    Line(batch,Ids[i]+"_CentreAndHeight",new[]{p+Vector3.up*.015f,p+Vector3.up*Heights[i]},mat);
                    var label=new GameObject(Ids[i]+"_ContractLabel").transform;label.SetParent(batch,false);label.position=p+new Vector3(0,Heights[i]+.10f,-.10f);label.rotation=c.transform.rotation;
                    var txt=label.gameObject.AddComponent<TextMesh>();txt.text=Ids[i].ToUpper()+" root / "+Footprints[i].x+" x "+Footprints[i].y+" m";txt.anchor=TextAnchor.MiddleCenter;txt.characterSize=.021f;txt.fontSize=48;txt.color=Color.cyan;txt.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");txt.GetComponent<MeshRenderer>().sharedMaterial=txt.font.material;
                }
            }
            for(int i=0;i<3;i++){var l=batch.Find(Ids[i]+"_ContractLabel");var p=GameObject.Find(Names[i]).transform.position;l.position=p+new Vector3(i==0?-.45f:i==1?.40f:0,i==1?.52f:1.05f,-.10f);l.GetComponent<TextMesh>().text=Ids[i].ToUpper()+"\\n"+Footprints[i].x+" x "+Footprints[i].y+" m";}
            guides.SetActive(false);AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            EditorSceneManager.OpenScene(SpriteRoomBuilder.ScenePath);SpriteRoomBuilder.Validate();
            Render(Camera.main,Out+"furniture_batch01_material_after.png",1600,900);
            Render(Camera.main,Out+"furniture_batch01_clean_1920.png",1920,1080);
            var g=EditorSceneManager.GetActiveScene().GetRootGameObjects().Single(x=>x.name=="ReviewGuides");g.SetActive(true);
            foreach(var name in Names)GameObject.Find(name).transform.Find("PlaceholderReference").gameObject.SetActive(false);
            try{Render(Camera.main,Out+"furniture_batch01_guide.png",1920,1080);}finally{g.SetActive(false);foreach(var name in Names)GameObject.Find(name).transform.Find("PlaceholderReference").gameObject.SetActive(true);}
            File.WriteAllText(Out+"furniture_batch01_PREPARED.txt","Prepared and reopened saved scene. Camera and architecture transforms unchanged. Exact roots preserved. Three Visual + ContactShadow containers prepared. Source textures unchanged; task-owned contrast derivatives only. All presentation remains unlit. READY designer package still required for art replacement.\n");
            Debug.Log("FURNITURE_BATCH01_PREPARED; contract and guides in "+Out);
        }
        [MenuItem("89.4/Studies/Sprite Room/Furniture Batch01 Capture Prepared")]
        public static void CapturePrepared()
        {
            if(EditorSceneManager.GetActiveScene().path!=SpriteRoomBuilder.ScenePath)throw new InvalidOperationException("Wrong scene");
            Render(Camera.main,Out+"furniture_batch01_material_after.png",1600,900);
            Render(Camera.main,Out+"furniture_batch01_clean_1920.png",1920,1080);
            var g=EditorSceneManager.GetActiveScene().GetRootGameObjects().Single(x=>x.name=="ReviewGuides");g.SetActive(true);
            foreach(var name in Names)GameObject.Find(name).transform.Find("PlaceholderReference").gameObject.SetActive(false);
            try{Render(Camera.main,Out+"furniture_batch01_guide.png",1920,1080);}finally{g.SetActive(false);foreach(var name in Names)GameObject.Find(name).transform.Find("PlaceholderReference").gameObject.SetActive(true);}
            Debug.Log("Furniture batch01 prepared capture complete");
        }        static void AddSprite(Transform root,string name,int order,Vector3 offset)
        {
            var t=new GameObject(name).transform;t.SetParent(root,false);t.localPosition=offset;t.rotation=Camera.main.transform.rotation;
            var s=t.gameObject.AddComponent<SpriteRenderer>();s.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");s.sortingOrder=order;s.enabled=false;
        }
        static void Quiet(string source,string dest,Color mean,float contrast)
        {
            if(File.Exists(Art+dest))return;
            var tex=new Texture2D(2,2,TextureFormat.RGB24,false);tex.LoadImage(File.ReadAllBytes(Art+source));var pixels=tex.GetPixels();
            // Deliberate non-destructive material input derivative; retains original texel phase and UV density.
            for(int i=0;i<pixels.Length;i++){var old=pixels[i];pixels[i]=new Color(mean.r+(old.r-.5f)*contrast,mean.g+(old.g-.5f)*contrast,mean.b+(old.b-.5f)*contrast,1);}
            tex.SetPixels(pixels);tex.Apply();File.WriteAllBytes(Art+dest,tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(Art+dest);
            var importer=(TextureImporter)AssetImporter.GetAtPath(Art+dest);importer.sRGBTexture=true;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.wrapMode=TextureWrapMode.Repeat;importer.mipmapEnabled=true;importer.SaveAndReimport();
        }
        static void SetMap(string material,string file)
        {
            var m=AssetDatabase.LoadAssetAtPath<Material>(Data+"Materials/"+material+".mat");m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Art+file));EditorUtility.SetDirty(m);
        }
        static void Line(Transform parent,string name,Vector3[] points,Material m)
        {
            var l=new GameObject(name).AddComponent<LineRenderer>();l.transform.SetParent(parent,false);l.sharedMaterial=m;l.positionCount=points.Length;l.SetPositions(points);l.startWidth=l.endWidth=.012f;
        }
        static void Render(Camera c,string path,int w,int h)
        {
            var old=c.targetTexture;var active=RenderTexture.active;var rt=RenderTexture.GetTemporary(w,h,24);
            try{c.targetTexture=rt;c.Render();RenderTexture.active=rt;var t=new Texture2D(w,h,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,w,h),0,0);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);}
            finally{c.targetTexture=old;RenderTexture.active=active;RenderTexture.ReleaseTemporary(rt);}
        }
    }
}


