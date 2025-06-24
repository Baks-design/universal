using System;
using Alchemy.Inspector;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    [Serializable]
    [CreateAssetMenu(menuName = "Data/Inventory/Item")]
    public class ItemDetails : ScriptableObject
    {
        [HorizontalGroup("BasicInfo")]
        [BoxGroup("BasicInfo/Left")]
        [Required]
        public string Name;
        [BoxGroup("BasicInfo/Left")]
        [Range(1, 999)]
        public int maxStack = 1;
        [BoxGroup("BasicInfo/Left")]
        [TextArea(3, 5)]
        public string Description;
        [BoxGroup("BasicInfo/Right")]
        [Preview(100)]
        [Required]
        public Sprite Icon;
        
        [BoxGroup("ID")]
        [ReadOnly]
        [LabelText("Unique ID")]
        public SerializableGuid Id = SerializableGuid.NewGuid();
        [BoxGroup("ID")]
        [Button()]
        [LabelText("Generate New ID")]
        public void AssignNewGuid() => Id = SerializableGuid.NewGuid();

        [FoldoutGroup("Advanced")]
        [Tooltip("Creates a new inventory item instance with this item's details")]
        public Item Create(int quantity) => new(this, quantity);
    }
}