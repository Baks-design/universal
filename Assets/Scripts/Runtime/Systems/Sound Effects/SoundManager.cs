using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.SoundEffects
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        [SerializeField] GameObject parentSounds;
        [SerializeField] AssetReference soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        IObjectPool<SoundEmitter> soundEmitterPool;
        GameObject soundInstantiated;

        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        void Start() => InitializePool();

        public SoundBuilder CreateSoundBuilder() => new(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound)
                return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }

            return true;
        }

        public SoundEmitter Get() => soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) => soundEmitterPool.Release(soundEmitter);

        public void StopAll()
        {
            for (var i = 0; i < activeSoundEmitters.Count; i++)
                activeSoundEmitters[i].Stop();

            FrequentSoundEmitters.Clear();
        }

        void InitializePool()
        => soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize);

        SoundEmitter CreateSoundEmitter()
        {
            soundInstantiated = Addressables
                .InstantiateAsync(soundEmitterPrefab, parentSounds.transform)
                .WaitForCompletion();
            soundInstantiated.SetActive(false);
            soundInstantiated.TryGetComponent<SoundEmitter>(out var sound);
            return sound;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            if (soundEmitter == null)
                return;

            Destroy(soundEmitter.gameObject);
        }
    }
}