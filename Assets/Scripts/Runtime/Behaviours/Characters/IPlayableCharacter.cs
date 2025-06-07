using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IPlayableCharacter
    {
        CharacterData Data { get; }
        Transform CharacterTransform { get; }
        Vector3 LastPosition { get; set; }
        Quaternion LastRotation { get; set; }

        void Activate();
        void Deactivate();
    }
}