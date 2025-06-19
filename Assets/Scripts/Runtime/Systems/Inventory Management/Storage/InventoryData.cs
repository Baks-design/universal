using System;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    [Serializable]
    public class InventoryData : ISaveable
    {
        public int Capacity;
        public int Coins;
        public Item[] Items;

        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}