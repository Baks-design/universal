using Alchemy.Inspector;
using UnityEngine;
using Universal.Runtime.Systems.EntityPersistence;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    public class InventoryPersistence : MonoBehaviour, IBind<InventoryData>
    {
        [SerializeField, ReadOnly] Inventory inventory;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(InventoryData data)
        {
            inventory.Controller.Bind(data);
            data.Id = Id;
        }
    }
}