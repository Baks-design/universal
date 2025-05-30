using Universal.Runtime.Systems.Persistence.Helpers;

namespace Universal.Runtime.Systems.Persistence.Interfaces
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}