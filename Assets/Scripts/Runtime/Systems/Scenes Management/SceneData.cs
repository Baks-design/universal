using System;
using Eflatun.SceneReference;

namespace Universal.Runtime.Systems.ScenesManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;
        
        public string Name => Reference.Name;
    }
}