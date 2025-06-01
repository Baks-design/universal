using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.Persistence.Data
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        [NonSerialized] public List<EntityData> EntityData = new();
        [NonSerialized] public int worldCount;
        [NonSerialized] public float globalTendency;
    }
}