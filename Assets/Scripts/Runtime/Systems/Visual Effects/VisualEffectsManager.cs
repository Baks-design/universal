using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.VisualEffects
{
    public class VisualEffectsManager : MonoBehaviour, IVisualEffectsServices 
    {
        [SerializeField] AssetReference effectEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        IObjectPool<EffectsEmitter> effectsEmitterPool;
        GameObject effectInstantiated;
        LinkedList<EffectsEmitter> frequentEffectEmitters = new();
        readonly List<EffectsEmitter> activeEffectEmitters = new();

        public LinkedList<EffectsEmitter> FrequentEffectEmitters
        {
            get => frequentEffectEmitters;
            set => frequentEffectEmitters = value;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IVisualEffectsServices>(this);
        }

        void Start() => InitializePool();

        public EffectsBuilder CreateSoundBuilder() => new(this);

        public bool CanPlaySound(EffectsData data)
        {
            if (!data.frequentEffect) return true;

            if (frequentEffectEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    frequentEffectEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("effectEmitter is already released");
                }
                return false;
            }

            return true;
        }

        public EffectsEmitter Get() => effectsEmitterPool.Get();

        public void ReturnToPool(EffectsEmitter effectEmitter) => effectsEmitterPool.Release(effectEmitter);

        public void StopAll()
        {
            for (var i = 0; i < activeEffectEmitters.Count; i++)
                activeEffectEmitters[i].Stop();

            frequentEffectEmitters.Clear();
        }

        void InitializePool()
        => effectsEmitterPool = new ObjectPool<EffectsEmitter>(
            CreateeffectEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize);

        EffectsEmitter CreateeffectEmitter()
        {
            effectInstantiated = Addressables
                .InstantiateAsync(effectEmitterPrefab, transform)
                .WaitForCompletion();
            effectInstantiated.SetActive(false);
            effectInstantiated.TryGetComponent<EffectsEmitter>(out var sound);
            return sound;
        }

        void OnTakeFromPool(EffectsEmitter effectEmitter)
        {
            effectEmitter.gameObject.SetActive(true);
            activeEffectEmitters.Add(effectEmitter);
        }

        void OnReturnedToPool(EffectsEmitter effectEmitter)
        {
            if (effectEmitter.Node != null)
            {
                frequentEffectEmitters.Remove(effectEmitter.Node);
                effectEmitter.Node = null;
            }
            effectEmitter.gameObject.SetActive(false);
            activeEffectEmitters.Remove(effectEmitter);
        }

        void OnDestroyPoolObject(EffectsEmitter effectEmitter)
        {
            if (effectEmitter == null) return;

            Destroy(effectEmitter.gameObject);
        }
    }
}