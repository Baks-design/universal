using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public List<EntityData> EntityData;
    }
}