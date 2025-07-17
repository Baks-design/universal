using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General Settings")]
        public AssetReferenceGameObject characterPrefab;

        [Header("Overshoot Settings")]
        [Range(0f, 1f)] public float overshootChance = 0.8f;
        [Range(0f, 1f)] public float overshootMinTriggerProgress = 0.7f;
        public Vector3 overshootAmount = new(2f, 1f, 0f);
        public float overshootRecoverySpeed = 3f;
        public bool enableOvershoot = true;
        public float overshootDistance = 0.25f;
        public AnimationCurve overshootCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        );

        [Header("Movement Settings")]
        public float gridSize = 2.5f;
        public float moveDuration = 0.6f;
        [Range(1f, 3f)] public float moveFactor = 1.5f; 
        public float smoothTime = 0.2f;
        public float maxSpeed = 2.5f;
        public float angularSpeedThreshold = 10f;
        public float minSpeedMultiplier = 0.5f;
        public float maxSpeedMultiplier = 2f;
        public AnimationCurve movementCurve = new(
            new Keyframe(0f, 0f, 0f, 0f), 
            new Keyframe(0.3f, 0.2f),  
            new Keyframe(0.8f, 0.9f),  
            new Keyframe(1f, 1f, 0f, 0f)   
        );

        [Header("Rotation Settings")]
        public float rotationDuration = 0.25f;
        public float angleNormalizationBase = 90f;
        public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Obstacle Collision Settings")]
        public LayerMask obstacleLayers;
        public float obstacleYOffset = 0.5f;
        public Vector3 collisionCheckSize = new(1.5f, 1.5f, 1.5f);

        [Header("Ground Collision Settings")]
        public LayerMask groundLayer;
        public float groundCheckDistance = 0.5f;

        [Header("Character Collision Settings")]
        public LayerMask detectionLayer;
        public float detectionRadius = 0.1f;
        public float detectionDistance = 2f;
    }
}