using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General Settings")]
        public AssetReferenceGameObject characterPrefab;

        [Header("Obstacle Collision Settings")]
        public LayerMask obstacleLayers = Physics.AllLayers;
        public float obstacleYOffset = 0.5f;
        public Vector3 collisionCheckSize = new(0.5f, 0.5f, 0.5f);
        [Header("Ground Collision Settings")]
        public LayerMask groundLayer = Physics.AllLayers;
        public float groundCheckDistance = 1f;
        public float groundCheckOffset = 1f;
        [Header("Character Collision Settings")]
        public LayerMask detectionLayer = Physics.AllLayers;
        public float detectionRadius = 0.3f;
        public float detectionDistance = 2f;

        [Header("Movement Settings")]
        public float gridSize = 3f;
        public AnimationCurve movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float moveDuration = 1f;
        public float rotationDuration = 1f;
    }
}