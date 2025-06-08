namespace Universal.Runtime.Systems.EntityPersistence
{
    public interface IPersistenceServices
    {
        void NewGame();
        void SaveGame();
        void LoadGame(string gameName);
        void ReloadGame();
        void DeleteGame(string gameName);
    }
}