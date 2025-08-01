namespace Universal.Runtime.Behaviours.Characters
{
    public interface ISpawnServices
    {
        ISpawnPoint GetSpawnPoint();
        void RegisterSpawnPoint(ISpawnPoint spawnPoint);
        void UnregisterSpawnPoint(ISpawnPoint spawnPoint);
    }
}