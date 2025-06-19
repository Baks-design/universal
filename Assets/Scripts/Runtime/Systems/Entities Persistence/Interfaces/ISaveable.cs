using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}