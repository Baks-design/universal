using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.VFX;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Systems.VisualEffects
{
    public class EffectsEmitter : MonoBehaviour
    {
        [SerializeField, Child] VisualEffect vfxSource;
        Coroutine playingCoroutine;
        IVisualEffectsServices visualEffectsServices;

        public EffectsData Data { get; private set; }
        public LinkedListNode<EffectsEmitter> Node { get; set; }

        void Start() => ServiceLocator.Global.Get(out visualEffectsServices);

        public void Initialize(EffectsData data) => Data = data;

        public void Play() => vfxSource.Play();

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            vfxSource.Stop();
            visualEffectsServices.ReturnToPool(this);
        }
    }
}