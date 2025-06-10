using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Setup")]
        public string characterName;
        public AssetReferenceGameObject characterPrefab;
        public Sprite characterIcon;

        [Header("Settings")]
        public float movementSpeed = 30f;
        public float speedChangeRate = 10f;
        [Range(0f, 0.3f)] public float rotationSmoothTime = 0.12f;

        [Header("Audio/Visual")]
        public AudioClip spawnSound;
        public VisualEffect spawnVFX;

        [Header("Abilities")]
        public SpecialAbility[] abilities;
    }
}