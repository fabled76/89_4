using System;
using System.IO;
using System.Linq;
using DeadAir.Studies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace DeadAir.Editor.Studies
{
    public static class SpriteLightLabBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/Studies/SpriteLightLab.unity";
        public const string Data = "Assets/_Game/Data/Studies/SpriteLightLab/";
        public const string Art = "Assets/_Game/Art/Studies/SpriteLightLab/";
        private const string FieldShader = "DeadAir/Studies/SpriteLightLab/SharedLightField";

        [MenuItem("89.4/Studies/Sprite Light Lab/Open Saved Lab")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) throw new FileNotFoundException("Create the task-owned lab explicitly first.");
            EditorSceneManager.OpenScene(ScenePath);
        }

        public static void Create()
        {
            if (File.Exists(ScenePath)) { EditorSceneManager.OpenScene(ScenePath); return; }
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            Directory.CreateDirectory(Data + "Meshes"); Directory.CreateDirectory(Data + "Materials"); AssetDatabase.Refresh();
            ImportSurfaceTextures();
            File.Copy(Data + "SourceShell.unity.txt", ScenePath); AssetDatabase.ImportAsset(ScenePath);
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var camera = Camera.main;
            var architecture = scene.GetRootGameObjects().Single(root => root.name == "Architecture");
            foreach (var root in scene.GetRootGameObjects())
                if (root != architecture && root.GetComponentInChildren<Camera>() == null) Object.DestroyImmediate(root);
            // Private meshes/materials/textures: parallel SpriteRoom editing cannot alter this lab.
            foreach (var filter in architecture.GetComponentsInChildren<MeshFilter>())
            {
                var original = filter.sharedMesh;
                var pb = filter.GetComponent<ProBuilderMesh>();
                if (pb != null) { pb.preserveMeshAssetOnDestroy = true; Object.DestroyImmediate(pb); }
                var copy = Object.Instantiate(original); copy.name = filter.name;
                AssetDatabase.CreateAsset(copy, Data + "Meshes/" + filter.name + ".asset"); filter.sharedMesh = copy;
                var collider = filter.GetComponent<MeshCollider>(); if (collider != null) collider.sharedMesh = copy;
                var renderer = filter.GetComponent<MeshRenderer>();
                var old = renderer.sharedMaterial;
                var material = NewMaterial(filter.name, false);
                if (old.HasProperty("_BaseColor")) material.SetColor("_Tint", old.GetColor("_BaseColor"));
                if (old.HasProperty("_BaseMap") && old.GetTexture("_BaseMap") != null)
                {
                    bool isFloor = filter.name.StartsWith("Floor");
                    material.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(Art + (isFloor ? "Wood_Quiet.png" : "Stucco_Quiet.png")));
                    var tiling = old.GetTextureScale("_BaseMap"); var offset = old.GetTextureOffset("_BaseMap");
                    material.SetVector("_UvTransform", new Vector4(tiling.x, tiling.y, offset.x, offset.y));
                }
                renderer.sharedMaterial = material; renderer.sortingOrder = -30; renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = false;
            }

            var lab = new GameObject("SpriteLightLab_Runtime");
            var field = lab.AddComponent<LabLightField>();
            field.warm = Source("Warm_Source", new Vector3(-1.25f, 1.7f, .1f), new Color(1,.83f,.63f), 1.65f, 2.7f);
            field.neon = Source("Neon_Source", new Vector3(1.35f, 1.25f, -.45f), new Color(.22f,.90f,1), 1.35f, 2.3f);

            var actor = new GameObject("LabActor_Physics"); actor.transform.position = new Vector3(.35f,.92f,-.15f);
            var body = actor.AddComponent<Rigidbody2D>(); body.gravityScale = 3; body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate; body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var box = actor.AddComponent<BoxCollider2D>(); box.size = new Vector2(.42f,1.8f);
            var physicsMaterial = new PhysicsMaterial2D("Lab_NoFriction") { friction = 0, bounciness = 0 };
            AssetDatabase.CreateAsset(physicsMaterial, Data + "Lab_NoFriction.physicsMaterial2D"); box.sharedMaterial = physicsMaterial;
            actor.AddComponent<PlayerController2D>();
            var movement = new SerializedObject(actor.GetComponent<PlayerController2D>()); movement.FindProperty("moveSpeed").floatValue = 1.45f; movement.ApplyModifiedPropertiesWithoutUndo();
            ImportHero();
            var visual = new GameObject("Hero_Selected_Static_REV02", typeof(SpriteRenderer)); visual.transform.SetParent(actor.transform,false);
            visual.transform.localPosition = new Vector3(0,-.9f,0); visual.transform.rotation = camera.transform.rotation;
            var hero = visual.GetComponent<SpriteRenderer>(); hero.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "Hero_Selected_Revision02.png");
            hero.sortingOrder = 20; var heroMaterial = NewMaterial("Hero_CurvedApproximation", true);
            heroMaterial.SetFloat("_Curvature", 1.25f); heroMaterial.SetVector("_ShapeUv", new Vector4(.4f,.704f,.045f,.958f)); hero.sharedMaterial = heroMaterial;
            hero.shadowCastingMode = ShadowCastingMode.Off;

            var sprite = ImportProxySprite();
            Proxy("Desk_SPRITE_PROXY",new Vector3(-.85f,0,.55f),new Vector2(1.35f,.74f),new Color(.50f,.36f,.25f));
            Proxy("Chair_SPRITE_PROXY",new Vector3(-.90f,0,.15f),new Vector2(.45f,.85f),new Color(.49f,.25f,.33f));
            Proxy("Bed_SPRITE_PROXY",new Vector3(2.05f,0,.65f),new Vector2(1.95f,.50f),new Color(.55f,.32f,.40f));
            Proxy("Receiver_SPRITE_PROXY",new Vector3(-.55f,.74f,.48f),new Vector2(.18f,.11f),new Color(.31f,.49f,.48f));
            // Small colored source markers are explicit review markers, not finished fixture art.
            Marker(field.warm); Marker(field.neon);

            var shadowObject = new GameObject("Soft_Floor_Contact_Only",typeof(MeshFilter),typeof(MeshRenderer));
            var mesh = new Mesh { name = "ContactFloorQuad" };
            mesh.vertices = new[] {new Vector3(-1,0,-1),new Vector3(-1,0,1),new Vector3(1,0,1),new Vector3(1,0,-1)};
            mesh.uv = new[] {Vector2.zero,Vector2.up,Vector2.one,Vector2.right}; mesh.triangles = new[] {0,1,2,0,2,3}; mesh.RecalculateNormals();
            AssetDatabase.CreateAsset(mesh,Data+"Meshes/ContactFloorQuad.asset");shadowObject.GetComponent<MeshFilter>().sharedMesh=mesh;
            var shadowMaterial = new Material(Shader.Find("DeadAir/Studies/SpriteLightLab/ContactOval"));
            AssetDatabase.CreateAsset(shadowMaterial,Data+"Materials/SoftContact.mat");shadowObject.GetComponent<MeshRenderer>().sharedMaterial=shadowMaterial;
            shadowObject.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;
            shadowObject.GetComponent<MeshRenderer>().sortingOrder=-20;
            var oval=shadowObject.AddComponent<LabContactOval>();oval.actor=actor.transform;oval.Apply();

            var physicsRoot=new GameObject("Lab_Path_Bounds_2D");var floor=physicsRoot.AddComponent<BoxCollider2D>();floor.size=new Vector2(6.4f,.2f);floor.offset=new Vector2(0,-.1f);
            foreach(float x in new[]{-3.3f,3.3f}){var wall=new GameObject("Bound_"+x);wall.transform.SetParent(physicsRoot.transform);wall.transform.position=new Vector3(x,1.4f,-.15f);wall.AddComponent<BoxCollider2D>().size=new Vector2(.2f,3);}
            var controls=lab.AddComponent<LabReviewControls>();controls.field=field;controls.actor=body;
            field.targets=architecture.GetComponentsInChildren<Renderer>().Concat(Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None).Where(r=>!r.name.StartsWith("SourceMarker"))).ToArray();
            field.Apply(); AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            Debug.Log("SpriteLightLab saved: task-owned dynamic shared field, static sprite, floor oval and bounded A/D; no native Light/pipeline changes.");

            LabLightSource Source(string name,Vector3 position,Color color,float intensity,float radius)
            {var go=new GameObject(name);go.transform.SetParent(lab.transform);go.transform.position=position;var light=go.AddComponent<LabLightSource>();light.color=color;light.intensity=intensity;light.range=radius;return light;}
            void Proxy(string name,Vector3 foot,Vector2 size,Color color)
            {var go=new GameObject(name,typeof(SpriteRenderer));go.transform.position=foot;go.transform.rotation=camera.transform.rotation;go.transform.localScale=new Vector3(size.x,size.y,1);var r=go.GetComponent<SpriteRenderer>();r.sprite=sprite;r.sortingOrder=-10;var m=NewMaterial(name,true);m.SetColor("_Tint",color);r.sharedMaterial=m;}
            void Marker(LabLightSource source)
            {var go=new GameObject("SourceMarker_"+source.name,typeof(SpriteRenderer));go.transform.SetParent(source.transform,false);go.transform.rotation=camera.transform.rotation;go.transform.localScale=new Vector3(.045f,.045f,1);var r=go.GetComponent<SpriteRenderer>();r.sprite=sprite;r.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");r.color=source.color;r.sortingOrder=30;go.SetActive(false);}
        }

        private static Material NewMaterial(string name,bool sprite)
        {
            var shader=Shader.Find(FieldShader);if(shader==null)throw new InvalidOperationException("Task local shader required.");
            var m=new Material(shader){name=name,renderQueue=sprite?3000:2000};m.SetFloat("_IsSprite",sprite?1:0);m.SetFloat("_ZWrite",sprite?0:1);m.SetFloat("_ZTest",sprite?8:4);m.SetVector("_UvTransform",new Vector4(1,1,0,0));
            AssetDatabase.CreateAsset(m,Data+"Materials/"+name+".mat");return m;
        }
        private static void ImportHero()
        {
            var t=(TextureImporter)AssetImporter.GetAtPath(Art+"Hero_Selected_Revision02.png");t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.spritePixelsPerUnit=331.03447f;
            t.sRGBTexture=true;t.mipmapEnabled=false;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;t.npotScale=TextureImporterNPOTScale.None;t.alphaIsTransparency=true;t.filterMode=FilterMode.Bilinear;
            var s=new TextureImporterSettings();t.ReadTextureSettings(s);s.spriteMeshType=SpriteMeshType.FullRect;s.spriteAlignment=(int)SpriteAlignment.Custom;s.spritePivot=new Vector2(.5f,30f/640);t.SetTextureSettings(s);t.SaveAndReimport();
        }
        private static Sprite ImportProxySprite()
        {
            const string path=Art+"Proxy_White.png";
            if(!File.Exists(path))
            {
                var white=new Texture2D(2,2,TextureFormat.RGBA32,false);white.SetPixels(Enumerable.Repeat(Color.white,4).ToArray());white.Apply();
                File.WriteAllBytes(path,white.EncodeToPNG());Object.DestroyImmediate(white);AssetDatabase.ImportAsset(path);
            }
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=2;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=new Vector2(.5f,0);importer.SetTextureSettings(settings);importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        private static void ImportSurfaceTextures()
        {
            foreach(string name in new[]{"Wood_Quiet.png","Stucco_Quiet.png"})
            {
                var t=(TextureImporter)AssetImporter.GetAtPath(Art+name);t.textureType=TextureImporterType.Default;t.wrapMode=TextureWrapMode.Repeat;t.sRGBTexture=true;t.textureCompression=TextureImporterCompression.Uncompressed;t.mipmapEnabled=true;t.maxTextureSize=2048;t.SaveAndReimport();
            }
        }
    }
}
