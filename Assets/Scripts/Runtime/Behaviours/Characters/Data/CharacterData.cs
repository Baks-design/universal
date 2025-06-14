using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General")]
        public string characterName;
        public AssetReferenceGameObject characterPrefab;

        [Header("Movement")]
        public float gridSize = 1f;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public LayerMask obstacleMask;

        [Header("Footsteps")]
        [Range(0.5f, 1f)] public float walkStepInterval = 0.5f;
        [Range(0.3f, 1f)] public float runStepInterval = 0.3f;
    }
}