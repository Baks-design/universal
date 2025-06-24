#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Universal.Runtime.Systems.ScenesManagement
{
    [CustomEditor(typeof(SceneLoaderManager))]
    public class SceneLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sceneLoader = (SceneLoaderManager)target;

            if (EditorApplication.isPlaying && GUILayout.Button("Load First Scene Group"))
                LoadSceneGroup(sceneLoader, 0);

            if (EditorApplication.isPlaying && GUILayout.Button("Load Second Scene Group"))
                LoadSceneGroup(sceneLoader, 1);
        }

        static async void LoadSceneGroup(SceneLoaderManager sceneLoader, int index)
        => await sceneLoader.LoadSceneGroup(index);
    }
}
#endif