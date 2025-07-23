using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    [Serializable]
    public class GameData
    {
        public string Name = "New Game";
        public string CurrentLevelName = "Demo";
        public List<EntityData> EntityData;
    }
}