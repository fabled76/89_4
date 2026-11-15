using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeadAir.Editor.Studies
{
    public static class ArtPaletteCPilotSetup
    {
        public const string ScenePath = "Assets/_Game/Scenes/Studies/ArtPilot_PaletteC.unity";

        [MenuItem("89.4/Studies/Open Palette C ArtPilot")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Prepare();
        }

        // Creates the isolated copy once. Existing pilot edits are never regenerated.
        public static void Prepare()
        {
            if (File.Exists(ScenePath)) { EditorSceneManager.OpenScene(ScenePath); return; }
            if (!File.Exists(VisualBlockoutBuilder.ScenePath)) throw new FileNotFoundException("Saved measured blockout required.");
            if (!AssetDatabase.CopyAsset(VisualBlockoutBuilder.ScenePath, ScenePath)) throw new IOException("Could not copy measured study.");
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var proxy = GameObject.Find("Receiver_ReviewProxy");
            Object.DestroyImmediate(proxy.GetComponent<InspectInteractable>());
            var toggle = proxy.AddComponent<ArtPilotReceiverToggle>();
            var data = new SerializedObject(toggle);
            data.FindProperty("receiverVisual").objectReferenceValue = GameObject.Find("MeasuredGeometry/Receiver");
            data.ApplyModifiedPropertiesWithoutUndo();
            new GameObject("ArtPilot_PaletteC_PENDING_DESIGNER_LAYERS");
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Debug.Log("Isolated ArtPilot prepared from saved meshes/camera. Neutral geometry only; designer layers and removable shadow remain pending.");
        }
    }
}
