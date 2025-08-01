using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class CharacterSoundSettings
    {
        [Range(0.1f, 1f)] public float crouchStepInterval = 0.8f;
        [Range(0.1f, 1f)] public float walkStepInterval = 0.5f;
        [Range(0.1f, 1f)] public float runStepInterval = 0.3f;
    }
}
