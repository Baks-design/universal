using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General Settings")]
        public AssetReferenceGameObject characterPrefab;

        [Header("Collision Settings")]
        public LayerMask obstacleLayers = Physics.AllLayers;
        public Vector3 collisionCheckSize = new(1.4f, 1f, 1.4f);
        public LayerMask groundLayer = Physics.AllLayers;
        public float groundCheckDistance = 1f;
        public LayerMask interactableLayer = Physics.AllLayers;
        public float interactDistance = 2f;

        [Header("Movement Settings")]
        public float gridSize = 3f;
        public AnimationCurve movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float movementSpeed = 1f;
        public float rotationSpeed = 5f;
    }
}