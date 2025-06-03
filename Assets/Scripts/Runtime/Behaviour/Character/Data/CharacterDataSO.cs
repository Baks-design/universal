using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Characters/Character Data")]
    public class CharacterDataSO : ScriptableObject
    {
        public string characterName;
        public Sprite icon;
        public float moveSpeed;
        public float abilityCooldown;
        public AbilityDataSO ability; // Single ability (expand to List if needed)
        public AssetReferenceGameObject prefab; // Reference to the character's prefab
        public Material dissolveMaterial; // Unique material for dissolve effects
    }
}