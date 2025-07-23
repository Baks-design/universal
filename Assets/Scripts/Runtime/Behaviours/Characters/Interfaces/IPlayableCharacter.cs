using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IPlayableCharacter
    {
        CharacterSettings Settings { get; }
        Transform CharacterTransform { get; }
        Vector3 LastPosition { get; set; }
        Quaternion LastRotation { get; set; }

        void Initialize(CharacterSettings settings);
    }
}