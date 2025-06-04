using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.VFX;

namespace Universal.Runtime.Systems.VisualEffects
{
    public class EffectsEmitter : MonoBehaviour
    {
        [SerializeField, Child] VisualEffect vfxSource;
        Coroutine playingCoroutine;

        public EffectsData Data { get; private set; }
        public LinkedListNode<EffectsEmitter> Node { get; set; }

        public void Initialize(EffectsData data) {  }

        public void Play() => vfxSource.Play();

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            vfxSource.Stop();
            VisualEffectsManager.Instance.ReturnToPool(this);
        }
    }
}