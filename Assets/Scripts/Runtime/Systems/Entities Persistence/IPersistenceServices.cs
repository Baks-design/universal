namespace Universal.Runtime.Systems.EntitiesPersistence
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