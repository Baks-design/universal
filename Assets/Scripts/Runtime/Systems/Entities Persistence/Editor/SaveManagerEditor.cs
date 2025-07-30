using UnityEditor;
using UnityEngine;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    [CustomEditor(typeof(PersistenceManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var saveLoadSystem = (PersistenceManager)target;
            var gameName = saveLoadSystem.name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game")) saveLoadSystem.NewGame();
            if (GUILayout.Button("Save Game")) saveLoadSystem.SaveGame();
            if (GUILayout.Button("Load Game")) saveLoadSystem.LoadGame(gameName);
            if (GUILayout.Button("Delete Game")) saveLoadSystem.DeleteGame(gameName);
        }
    }
}