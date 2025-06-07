using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Character System/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Basic")]
        public string characterName;
        public AssetReferenceGameObject characterPrefab;
        public Sprite characterIcon;

        [Header("Abilities")]
        public float movementSpeed = 5f;
        public float jumpForce = 7f;
        public SpecialAbility[] abilities;

        [Header("Audio/Visual")]
        public AudioClip spawnSound;
        public VisualEffect spawnVFX;
    }
}