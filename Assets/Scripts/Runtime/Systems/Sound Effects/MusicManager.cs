using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using static Freya.Mathfs;

namespace Universal.Runtime.Systems.SoundEffects
{
    public class MusicManager : MonoBehaviour, IMusicServices
    {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        float fading;
        AudioSource current;
        AudioSource previous;
        const float crossFadeTime = 1f;
        readonly Queue<AudioClip> playlist = new();

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IMusicServices>(this);
        }

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
            if (current && current.clip == clip) return;

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

        void Update()
        {
            HandleCrossFade(Time.deltaTime);

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        void HandleCrossFade(float deltaTime)
        {
            if (fading <= 0f) return;

            fading += deltaTime;

            var fraction = Clamp01(fading / crossFadeTime);

            float logFraction = fraction.ToLogarithmicFraction();

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