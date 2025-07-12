using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General Settings")]
        public AssetReferenceGameObject characterPrefab;

        [Header("Movement Settings")]
        public float gridSize = 3f;
        public AnimationCurve movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float moveDuration = 2.5f;
        public float rotationDuration = 2.5f;

        [Header("Obstacle Collision Settings")]
        public LayerMask obstacleLayers;
        public float obstacleYOffset = 0.5f;
        public Vector3 collisionCheckSize = new(0.5f, 0.5f, 0.5f);

        [Header("Ground Collision Settings")]
        public LayerMask groundLayer;
        public float groundCheckDistance = 1f;

        [Header("Character Collision Settings")]
        public LayerMask detectionLayer;
        public float detectionRadius = 0.3f;
        public float detectionDistance = 2f;
    }
}