using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using Universal.Runtime.Systems.EntitiesPersistence;
using Universal.Runtime.Systems.VisualEffects;
using Universal.Runtime.Systems.DamageObjects;
using Universal.Runtime.Utilities.Tools.Updates;

namespace Universal.Runtime.Systems.StatesManagement
{
    public class ServicesBootstrapper : MonoBehaviour
    {
        [SerializeField] ServiceConfiguration services;

        void Awake()
        {
            Instantiate(services.GameStateManager);
            Instantiate(services.SceneLoaderManager);
            Instantiate(services.InputReaderManager);
            var serviceLocator = ServiceLocator.Global;
            serviceLocator.Register<IUpdateManager>(Instantiate(services.UpdateManager));
            serviceLocator.Register<IMusicServices>(Instantiate(services.MusicManager));
            serviceLocator.Register<ISoundEffectsServices>(Instantiate(services.SoundManager));
            serviceLocator.Register<IVisualEffectsServices>(Instantiate(services.VisualManager));
            serviceLocator.Register<IPersistenceServices>(Instantiate(services.PersistenceManager));
            serviceLocator.Register<ILifeCurrencyServices>(Instantiate(services.LifeCurrencyManager));
        }
    }
}