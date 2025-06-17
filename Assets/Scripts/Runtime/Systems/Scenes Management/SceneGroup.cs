using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }

    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            for (var i = 0; i < Scenes.Count; i++)
            {
                var scene = Scenes[i];
                if (scene.SceneType == sceneType)
                    return scene.Reference.Name;
            }

            return null;
        }
    }
}