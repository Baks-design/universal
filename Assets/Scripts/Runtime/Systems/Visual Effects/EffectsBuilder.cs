using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Systems.VisualEffects
{
    public class EffectsBuilder
    {
        Vector3 position = Vector3.zero;
        readonly VisualEffectsManager visualEffectsManager;

        public EffectsBuilder(VisualEffectsManager visualEffectsManager)
        => this.visualEffectsManager = visualEffectsManager;

        public EffectsBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
 
        public void Play(EffectsData effectsData)
        {
            if (effectsData == null)
            {
                Logging.LogError("EffectsData is null");
                return;
            }

            if (!visualEffectsManager.CanPlaySound(effectsData))
                return;

            var effectEmitter = visualEffectsManager.Get();
            effectEmitter.Initialize(effectsData);
            effectEmitter.transform.position = position;
            effectEmitter.transform.parent = visualEffectsManager.transform;

            if (effectsData.frequentEffect)
                effectEmitter.Node = visualEffectsManager.FrequentEffectEmitters.AddLast(effectEmitter);

            effectEmitter.Play();
        }
    }
}