using System;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.EntitiesPersistence
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