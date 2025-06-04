using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterDataSO : ScriptableObject
    {
        public string characterName;
        public Sprite icon;
        public float moveSpeed;
        public float abilityCooldown;
        public float abilityMultiplier;
        public float abilityDuration;
        public AbilityDataSO ability; // Single ability (expand to List if needed)
        public AssetReferenceGameObject prefab; // Reference to the character's prefab
        public Material dissolveMaterial; // Unique material for dissolve effects
    }
}