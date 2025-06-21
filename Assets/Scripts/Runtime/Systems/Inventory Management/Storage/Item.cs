using System;
using Alchemy.Inspector;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    [Serializable]
    public class Item
    {
        [SerializeField] public SerializableGuid Id;
        [SerializeField] public SerializableGuid detailsId;
        [InlineEditor] public ItemDetails details;
        [NonSerialized] public int quantity;

        public Item(ItemDetails details, int quantity = 1)
        {
            detailsId = details.Id;
            this.details = details;
            this.quantity = quantity;

            Id = SerializableGuid.NewGuid();
        }
    }
}