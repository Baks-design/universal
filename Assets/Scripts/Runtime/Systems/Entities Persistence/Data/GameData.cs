using System;
using System.Collections.Generic;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public int worldCount;
        public float globalTendency;
        public InventoryData inventoryData;
        public List<EntityData> EntityData;
    }
}