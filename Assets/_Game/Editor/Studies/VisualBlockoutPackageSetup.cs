using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace DeadAir.Editor.Studies
{
    public static class VisualBlockoutPackageSetup
    {
        private static AddRequest request;
        public static void InstallReleasedProBuilder()
        {
            request = Client.Add("com.unity.probuilder@6.1.4");
            EditorApplication.update += Poll;
        }
        private static void Poll()
        {
            if (!request.IsCompleted) return;
            EditorApplication.update -= Poll;
            if (request.Status == StatusCode.Success && request.Result.version == "6.1.4")
            {
                Debug.Log("Study dependency installed through Package Manager: " + request.Result.name + "@" + request.Result.version);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("ProBuilder installation did not complete: " + request.Error?.message);
                EditorApplication.Exit(1);
            }
        }
    }
}
