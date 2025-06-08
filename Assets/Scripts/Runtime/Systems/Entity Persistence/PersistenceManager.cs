using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Universal.Runtime.Systems.Persistence.Interfaces;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.EntityPersistence
{
    public class PersistenceManager : MonoBehaviour, IPersistenceServices
    {
        [SerializeField] GameData gameData;
        IDataService dataService;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IPersistenceServices>(this);
            dataService = new FileDataService(new JsonSerializer());
        }

        void Start() => NewGame();

        void OnApplicationQuit() => SaveGame();

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(GlobalTags.MenuString)) return;
            Bind<EntityPersistence, EntityData>(gameData.EntityData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            T entity = null;
            if (entities.Length > 0)
                entity = entities[0];
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                TData data = default;
                for (var j = 0; j < datas.Count; j++)
                {
                    var d = datas[j];
                    if (d.Id == entity.Id)
                    {
                        data = d;
                        break;
                    }
                }
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = GlobalTags.GameString,
                CurrentLevelName = GlobalTags.DemoString,
                EntityData = new List<EntityData>(),
            };
            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);
            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName))
                gameData.CurrentLevelName = GlobalTags.DemoString;
            SceneManager.LoadScene(gameData.CurrentLevelName);

            
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}