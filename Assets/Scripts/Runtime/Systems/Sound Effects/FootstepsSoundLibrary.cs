using UnityEngine;

namespace Universal.Runtime.Systems.SoundEffects
{
    [CreateAssetMenu(menuName = "Data/Audio/Footsteps")]
    public class FootstepsSoundLibrary : ScriptableObject
    {
        public SurfaceType surfaceType;
        public SoundData[] footstepSounds;
        public SoundData[] landSounds;
    }
}
