using System;
using Universal.Runtime.Systems.SoundEffects;
using Universal.Runtime.Systems.EntitiesPersistence;
using Universal.Runtime.Systems.VisualEffects;
using Universal.Runtime.Systems.DamageObjects;
using Universal.Runtime.Systems.ScenesManagement;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.Updates;

namespace Universal.Runtime.Systems.StatesManagement
{
    [Serializable]
    public struct ServiceConfiguration
    {
        public UpdateManager UpdateManager;
        public MusicManager MusicManager;
        public SoundEffectsManager SoundManager;
        public VisualEffectsManager VisualManager;
        public PersistenceManager PersistenceManager;
        public GameStateManager GameStateManager;
        public LifeCurrencyManager LifeCurrencyManager;
        public SceneLoaderManager SceneLoaderManager;
        public InputReaderManager InputReaderManager;
    }
}