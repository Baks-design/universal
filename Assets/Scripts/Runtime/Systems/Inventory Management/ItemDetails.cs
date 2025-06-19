using System;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    [CreateAssetMenu(menuName = "Data/Inventory/Item")]
    [Serializable]
    public class ItemDetails : ScriptableObject
    {
        [BeginHorizontalGroup(Label = "ItemSplit", LabelWidth = 0.75f)]
        public string Name;
        public int maxStack = 1;
        public SerializableGuid Id = SerializableGuid.NewGuid();
        [AssetPreview] public Sprite Icon;
        [EndHorizontalGroup]
        [TextArea, HideLabel] public string Description;

        public Item Create(int quantity) => new(this, quantity);
    }
}