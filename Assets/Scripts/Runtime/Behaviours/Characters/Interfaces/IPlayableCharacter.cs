using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IPlayableCharacter
    {
        Grid Grid { get; set; }
        CharacterData CharacterData { get; }
        Transform CharacterTransform { get; }
        Vector3 LastPosition { get; set; }
        Quaternion LastRotation { get; set; }

        void Initialize(CharacterData data);
    }
}