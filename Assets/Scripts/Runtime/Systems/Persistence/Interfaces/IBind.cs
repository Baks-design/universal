using Universal.Runtime.Systems.Persistence.Helpers;

namespace Universal.Runtime.Systems.Persistence.Interfaces
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }

        void Bind(TData data);
    }
}