using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Character/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General")]
        public AssetReferenceGameObject characterPrefab;

        [Header("Movement")]
        public LayerMask obstacleLayers;
        public float gridSize = 1f;
        public float rotationSpeed = 10f;

        [Header("Bob Settings")]
        [Range(14f, 16f)] public float walkBobSpeed = 14f;
        [Range(0.03f, 0.07f)] public float walkBobAmount = 0.05f;
        [Range(18f, 22f)] public float runBobSpeed = 18f;
        [Range(0.08f, 0.15f)] public float runBobAmount = 0.1f;
        public float transitionSpeed = 10f;

        [Header("Landing Effects")]
        [Range(0.15f, 0.3f)] public float landBobAmount = 0.2f;
        [Range(5f, 10f)] public float landRecoverySpeed = 5f;

        [Header("Rotation Effects")]
        public float tiltAmount = 3f;
        public float tiltSpeed = 8f;

        [Header("Landing Detection")]
        public LayerMask groundLayers;
        public float groundCheckDistance = 0.2f;
        public float groundSphereRadius = 0.2f;
        [Range(0.3f, 1f)] public float fallThreshold = 0.5f; 

        [Header("Footsteps")]
        public LayerMask floorMask;
        public float footstepsRayDistance = 2f;
        public float footstepThreshold = 0.1f;
    }
}