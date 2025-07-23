using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Settings/Character")]
    public class CharacterSettings : ScriptableObject
    {
        [Header("General Settings")]
        public AssetReferenceGameObject characterPrefab;
    }
}