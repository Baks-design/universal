namespace Universal.Runtime.Systems.EntitiesPersistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}