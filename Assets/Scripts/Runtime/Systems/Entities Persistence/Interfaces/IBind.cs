using Universal.Runtime.Systems.EntityPersistence;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }

        void Bind(TData data);
    }
}