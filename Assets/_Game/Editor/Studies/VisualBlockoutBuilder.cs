using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace DeadAir.Editor.Studies
{
    public static class VisualBlockoutBuilder
    {
        public const string ScenePath = "Assets/_Game/Scenes/Studies/VisualBlockout.unity";
        public const string Data = "Assets/_Game/Data/Studies/VisualBlockout/";
        public const float PathZ = .25f;
        public const float CameraPitch = 18f;
        public static readonly Vector3 CameraTarget = new Vector3(0, 1.2f, PathZ);
        private static Material[] shades;

        [MenuItem("89.4/Studies/Open Measured Visual Blockout")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Create();
        }
        public static void Create()
        {
            if (File.Exists(ScenePath)) { EditorSceneManager.OpenScene(ScenePath); return; }
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            Directory.CreateDirectory(Data + "Meshes"); Directory.CreateDirectory(Data + "Materials"); AssetDatabase.Refresh();
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) throw new InvalidOperationException("Existing URP Unlit shader required; no renderer migration allowed.");
            float[] values = { .80f, .67f, .55f, .43f, .29f };
            shades = values.Select((value, i) =>
            {
                string path = Data + "Materials/Neutral_" + i + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    material = new Material(shader) { name = "Neutral_" + i };
                    material.SetColor("_BaseColor", new Color(value, value, value)); AssetDatabase.CreateAsset(material, path);
                }
                return material;
            }).ToArray();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var geometry = new GameObject("MeasuredGeometry");
            var room = Group("Room", geometry.transform);
            // Nominal room is 6.4 x 3.0. A coplanar cutaway apron extends to the viewport edge, not a platform face.
            Cube("Floor_WithPresentationApron", room, new Vector3(0, -.05f, -1.5f), new Vector3(8.8f, .1f, 6.1f));
            Cube("Wall_Left", room, new Vector3(-3.25f, 1.3f, 0), new Vector3(.1f, 2.6f, 3));
            Cube("Wall_Right", room, new Vector3(3.25f, 1.3f, 0), new Vector3(.1f, 2.6f, 3));
            Cube("Wall_Back_LeftPier", room, new Vector3(-2.9625f, 1.3f, 1.55f), new Vector3(.475f, 2.6f, .1f));
            Cube("Wall_Back_Main", room, new Vector3(.7125f, 1.3f, 1.55f), new Vector3(4.975f, 2.6f, .1f));
            Cube("Wall_Back_DoorHeader", room, new Vector3(-2.25f, 2.4f, 1.55f), new Vector3(.95f, .4f, .1f));
            var door = Group("Door", geometry.transform);
            Cube("Door_Leaf", door, new Vector3(-2.25f, 1.05f, 1.5f), new Vector3(.85f, 2.10f, .04f));
            Cube("Door_Frame_Left", door, new Vector3(-2.70f, 1.1f, 1.47f), new Vector3(.05f, 2.20f, .08f));
            Cube("Door_Frame_Right", door, new Vector3(-1.80f, 1.1f, 1.47f), new Vector3(.05f, 2.20f, .08f));
            Cube("Door_Frame_Top", door, new Vector3(-2.25f, 2.175f, 1.47f), new Vector3(.95f, .05f, .08f));
            Cube("Door_Handle", door, new Vector3(-1.95f, 1.0f, 1.435f), new Vector3(.10f, .025f, .04f));
            var desk = Group("Desk", geometry.transform);
            Cube("Desk_Top", desk, new Vector3(-.4f, .72f, 1.1f), new Vector3(1.35f, .04f, .65f));
            foreach (float x in new[] { -1.015f, .215f }) foreach (float z in new[] { .835f, 1.365f })
                Cube("Desk_Leg_" + Sign(x) + "_" + Sign(z - 1.1f), desk, new Vector3(x, .35f, z), new Vector3(.05f, .70f, .05f));
            var chair = Group("Chair", geometry.transform);
            Cube("Chair_Seat", chair, new Vector3(-.4f, .43f, .62f), new Vector3(.45f, .04f, .45f));
            foreach (float x in new[] { -.585f, -.215f }) foreach (float z in new[] { .435f, .805f })
                Cube("Chair_Leg_" + Sign(x + .4f) + "_" + Sign(z - .62f), chair, new Vector3(x, .205f, z), new Vector3(.04f, .41f, .04f));
            Cube("Chair_Back", chair, new Vector3(-.4f, .65f, .415f), new Vector3(.45f, .40f, .04f));
            var bed = Group("Bed", geometry.transform);
            Cube("Bed_Mattress", bed, new Vector3(2, .41f, 1), new Vector3(1.95f, .18f, .90f));
            Cube("Bed_Base", bed, new Vector3(2, .27f, 1), new Vector3(1.95f, .10f, .90f));
            foreach (float x in new[] { 1.125f, 2.875f }) foreach (float z in new[] { .65f, 1.35f })
                Cube("Bed_Leg_" + Sign(x - 2) + "_" + Sign(z - 1), bed, new Vector3(x, .11f, z), new Vector3(.10f, .22f, .10f));
            Cube("Bed_Headboard", bed, new Vector3(2.995f, .45f, 1), new Vector3(.04f, .90f, .90f));
            var bag = Group("TravelBag", geometry.transform);
            Cube("Bag_Body", bag, new Vector3(-1.35f, .12f, .75f), new Vector3(.45f, .24f, .25f));
            foreach (float x in new[] { -1.43f, -1.27f })
                Cube("Bag_HandlePost_" + Sign(x + 1.35f), bag, new Vector3(x, .265f, .75f), new Vector3(.02f, .05f, .04f));
            Cube("Bag_HandleTop", bag, new Vector3(-1.35f, .29f, .75f), new Vector3(.18f, .02f, .04f));
            var receiver = Group("Receiver", geometry.transform);
            Cube("Receiver_Body", receiver, new Vector3(-.4f, .795f, .85f), new Vector3(.18f, .11f, .05f));
            Cube("Receiver_Display", receiver, new Vector3(-.405f, .81f, .822f), new Vector3(.115f, .035f, .006f), 4);
            Cube("Receiver_Antenna", receiver, new Vector3(-.33f, .975f, .85f), new Vector3(.006f, .25f, .006f));
            var calibration = Group("Calibration_OutsideFrame", geometry.transform);
            Cube("Calibration_1m_Cube", calibration, new Vector3(5, .5f, 0), Vector3.one);

            var person = Group("ReferencePerson", geometry.transform);
            person.position = new Vector3(.9f, .9f, PathZ);
            Cube("Person_Torso", person, new Vector3(.9f, 1.245f, PathZ), new Vector3(.40f, .55f, .20f));
            Cube("Person_Pelvis", person, new Vector3(.9f, .885f, PathZ), new Vector3(.31f, .17f, .22f));
            Cube("Person_Neck", person, new Vector3(.9f, 1.54f, PathZ), new Vector3(.10f, .08f, .10f));
            foreach (float side in new[] { -1f, 1f })
            {
                Cube("Person_Leg_" + Sign(side), person, new Vector3(.9f + side * .095f, .455f, PathZ), new Vector3(.12f, .69f, .16f));
                Cube("Person_Foot_" + Sign(side), person, new Vector3(.9f + side * .095f, .055f, PathZ - .03f), new Vector3(.13f, .11f, .28f));
                Cube("Person_Arm_" + Sign(side), person, new Vector3(.9f + side * .27f, 1.215f, PathZ), new Vector3(.10f, .55f, .14f));
                Cube("Person_Hand_" + Sign(side), person, new Vector3(.9f + side * .27f, .9f, PathZ), new Vector3(.10f, .12f, .12f));
            }
            var head = ShapeGenerator.GenerateIcosahedron(PivotLocation.Center, .12f, 2);
            head.name = "Person_Head"; head.transform.SetParent(person, true);
            head.transform.position = new Vector3(.9f, 1.8f - head.GetComponent<MeshFilter>().sharedMesh.bounds.max.y, PathZ);
            Finish(head);
            var body = person.gameObject.AddComponent<Rigidbody2D>(); body.gravityScale = 3;
            body.constraints = RigidbodyConstraints2D.FreezeRotation; body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var personCollider = person.gameObject.AddComponent<BoxCollider2D>(); personCollider.size = new Vector2(.65f, 1.8f);
            personCollider.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>("Assets/_Game/Data/CorridorPlayer.physicsMaterial2D");
            person.gameObject.AddComponent<PlayerController2D>(); var interaction = person.gameObject.AddComponent<InteractionController>();
            new SerializedObject(interaction).FindProperty("radius"); // Runtime default reach stays unchanged.
            var floorPhysics = Group("HorizontalPathPhysics_2DOnly", null);
            var ground = floorPhysics.gameObject.AddComponent<BoxCollider2D>(); ground.size = new Vector2(6.4f, .2f); ground.offset = new Vector2(0, -.1f);
            foreach (float x in new[] { -3.3f, 3.3f })
            {
                var wall = Group("PathLimit_" + Sign(x), floorPhysics); wall.position = new Vector3(x, 1.5f, PathZ);
                wall.gameObject.AddComponent<BoxCollider2D>().size = new Vector2(.2f, 3);
            }
            Proxy("Door_TransitionProxy", new Vector3(-2.25f, 1.05f, PathZ), new Vector2(.85f, 2.10f), "Study door transition proxy; no literal 3D handle reach is claimed.");
            Proxy("Receiver_ReviewProxy", new Vector3(-.4f, .795f, PathZ), new Vector2(.18f, .11f), "Study receiver interaction reached.");
            Proxy("Bag_InspectProxy", new Vector3(-1.35f, .15f, PathZ), new Vector2(.45f, .3f), "Study bag interaction reached.");
            var camera = new GameObject("BlockoutCamera", typeof(Camera)).GetComponent<Camera>(); camera.tag = "MainCamera";
            SetCamera(camera, CameraPitch); camera.orthographic = true; camera.orthographicSize = 2.25f; camera.aspect = 16f / 9f;
            camera.nearClipPlane = .1f; camera.farClipPlane = 30; camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.25f, .25f, .25f); camera.allowHDR = false; camera.allowMSAA = false;
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("Measured VisualBlockout created separately. Existing game scenes/startup/renderer unchanged.");
        }
        public static void SetCamera(Camera camera, float pitch)
        {
            camera.transform.rotation = Quaternion.Euler(pitch, 0, 0);
            camera.transform.position = CameraTarget - camera.transform.forward * 10;
        }
        private static Transform Group(string name, Transform parent)
        {
            var go = new GameObject(name); if (parent != null) go.transform.SetParent(parent, true); return go.transform;
        }
        private static void Cube(string name, Transform parent, Vector3 center, Vector3 size, int materialOverride = -1)
        {
            var pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size); pb.name = name;
            pb.transform.SetParent(parent, true); pb.transform.position = center; Finish(pb, materialOverride);
        }
        private static void Finish(ProBuilderMesh pb, int materialOverride = -1)
        {
            foreach (var face in pb.faces)
            {
                var indexes = face.indexes;
                var normal = Vector3.Cross(pb.positions[indexes[1]] - pb.positions[indexes[0]], pb.positions[indexes[2]] - pb.positions[indexes[0]]).normalized;
                face.submeshIndex = materialOverride >= 0 ? materialOverride : normal.y > .5f ? 0 : normal.z < -.5f ? 1 : Mathf.Abs(normal.x) > .5f ? 2 : 3;
            }
            pb.GetComponent<MeshRenderer>().sharedMaterials = shades;
            pb.ToMesh(); pb.Refresh(); pb.preserveMeshAssetOnDestroy = true;
            var mesh = pb.GetComponent<MeshFilter>().sharedMesh; mesh.hideFlags = HideFlags.None;
            string path = Data + "Meshes/" + pb.name + ".asset";
            AssetDatabase.CreateAsset(mesh, path);
            var data = new SerializedObject(pb); data.FindProperty("assetGuid").stringValue = AssetDatabase.AssetPathToGUID(path); data.ApplyModifiedPropertiesWithoutUndo();
        }
        private static void Proxy(string name, Vector3 center, Vector2 size, string message)
        {
            var go = new GameObject(name); go.transform.position = center;
            var collider = go.AddComponent<BoxCollider2D>(); collider.isTrigger = true; collider.size = size;
            var inspect = go.AddComponent<InspectInteractable>(); var data = new SerializedObject(inspect);
            data.FindProperty("message").stringValue = message; data.ApplyModifiedPropertiesWithoutUndo();
        }
        private static string Sign(float value) => value < 0 ? "L" : "R";
    }
}
