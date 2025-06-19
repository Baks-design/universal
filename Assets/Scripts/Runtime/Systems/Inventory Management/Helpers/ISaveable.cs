using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}