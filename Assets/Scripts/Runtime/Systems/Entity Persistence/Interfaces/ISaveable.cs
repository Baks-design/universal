namespace Universal.Runtime.Systems.EntityPersistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}