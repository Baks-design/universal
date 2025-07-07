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
        public LayerMask obstacleLayers;
        public float gridSize = 1f;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        [Header("Crouch Settings")]
        public LayerMask ceilingCheckMask;
        [Range(0.1f, 0.9f)] public float crouchCameraHeightRatio = 0.6f;
        public float crouchTransitionSpeed = 2f;

        [Header("Bob Settings")]
        [Range(14f, 16f)] public float walkBobSpeed = 14f;
        [Range(0.03f, 0.07f)] public float walkBobAmount = 0.05f;
        public float transitionSpeed = 10f;
        [Range(0.15f, 0.3f)] public float landBobAmount = 0.2f;

        [Header("Ground Detection Settings")]
        public LayerMask groundLayers;
        public float groundCheckDistance = 0.2f;
        public float groundSphereRadius = 0.2f;
        [Range(0.3f, 1f)] public float fallThreshold = 0.5f;

        [Header("Character Detection Settings")]
        public LayerMask characterLayer;
        public float raySphereRadius = 0.3f;
        public float raySphereMaxDistance = 2f;

        [Header("Sound Settings")]
        public float footstepThreshold = 0.1f;
    }
}