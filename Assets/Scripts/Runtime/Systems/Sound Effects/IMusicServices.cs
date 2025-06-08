using UnityEngine;

namespace Universal.Runtime.Systems.SoundEffects
{
    public interface IMusicServices
    {
        void AddToPlaylist(AudioClip clip);
        void PlayNextTrack();
        void Clear();
    }
}