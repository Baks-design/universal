using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Audio")]
    public class CharacterAudioData : ScriptableObject
    {
        [Range(0.5f, 1f)] public float walkStepInterval = 0.5f;
        [Range(0.3f, 1f)] public float runStepInterval = 0.3f;
    }
}
