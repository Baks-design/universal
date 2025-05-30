namespace Universal.Runtime.Systems.Persistence.Interfaces
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}