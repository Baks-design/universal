using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        [NonSerialized] public InventoryData inventoryData;
        [NonSerialized] public List<EntityData> EntityData = new();
    }
}