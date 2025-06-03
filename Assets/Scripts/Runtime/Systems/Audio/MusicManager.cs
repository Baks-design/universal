using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Universal.Runtime.Systems.ManagedUpdate;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.Audio
{
    public class MusicManager : PersistentSingleton<MusicManager>, IUpdatable
    {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        float fading;
        AudioSource current;
        AudioSource previous;
        const float crossFadeTime = 1f;
        readonly Queue<AudioClip> playlist = new();

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (current == null && previous == null)
                PlayNextTrack();
        }

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out var nextTrack))
                Play(nextTrack);
        }
        
        public void Clear() => playlist.Clear();

        void Play(AudioClip clip)
        {
            if (current && current.clip == clip)
                return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;

            gameObject.TryGetComponent(out AudioSource source);
            current = source;
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup; // Set mixer group
            current.loop = false; // For playlist functionality, we want tracks to play once
            current.volume = 0f;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }

        void IUpdatable.ManagedUpdate(float deltaTime)
        {
            HandleCrossFade(deltaTime);

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        void HandleCrossFade(float deltaTime)
        {
            if (fading <= 0f) return;

            fading += deltaTime;

            var fraction = Mathf.Clamp01(fading / crossFadeTime);

            var logFraction = fraction.ToLogarithmicFraction();

            if (previous)
                previous.volume = 1f - logFraction;
            if (current)
                current.volume = logFraction;

            if (fraction >= 1f)
            {
                fading = 0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}