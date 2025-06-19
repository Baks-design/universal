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

        [Header("Input")]
        [Range(0f, 1f)] public float inputCooldown = 0.1f;
        [Min(0f)] public float inputResponseSpeed = 10f;
        [Min(0f)] public float inputDeceleration = 15f;
        [Min(0f)] public float moveQueueThreshold = 1f;

        [Header("Movement")]
        public LayerMask obstacleMask;
        [Range(0f, 1f)] public float moveDuration = 0.1f;
        [Range(0.1f, 0.5f)] public float obstacleCheckRadius = 0.5f;
        [Range(0.1f, 1f)] public float rotateDuration = 0.35f;
        [AnimationCurveSettings]
        public AnimationCurve moveCurve = new(
            new Keyframe(0f, 0f, 1.5f, 1.5f),  // Smooth acceleration
            new Keyframe(0.7f, 1.1f, 0.5f, 0.5f),  // Slight overshoot
            new Keyframe(1f, 1f, 0f, 0f) // Perfect landing
        );

        [Header("Footsteps")]
        public LayerMask floorMask;
        [Range(0.5f, 1f)] public float footstepsDistance = 1f;
        [Range(0.1f, 1f)] public float walkStepInterval = 0.5f;
    }
}