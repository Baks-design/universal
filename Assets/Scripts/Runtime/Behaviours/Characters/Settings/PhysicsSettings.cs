using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class PhysicsSettings
    {
        [Header("Ground Settings")]
        public LayerMask groundLayers = ~0;
        [Range(0.1f, 1f)] public float rayGroundLength = 0.1f;

        [Header("Obstacles Settings")]
        public LayerMask obstacleLayers = ~0;
        [Range(0.1f, 1f)] public float rayObstacleLength = 0.4f;
        [Range(0.01f, 1f)] public float rayObstacleSphereRadius = 0.2f;

        [Header("Roof Settings")]
        public LayerMask roofLayers = ~0;

        [Header("Push Physics")]
        public float pushPower = 5f;
        public ForceMode pushForceMode = ForceMode.Impulse;
        public float maxPushedVelocity = 10f;
        public bool useCharacterForward = false;
        [Range(0f, 1f)] public float verticalInfluence = 0f;
        public LayerMask pushableLayers = ~0; 
        public bool scalePowerByMass = true;
        public float minMass = 0.5f;
        public float maxMass = 5f;
        public bool applyTorque = false;
        public float torqueMultiplier = 0.1f;
        public bool enablePushCooldown = false;
        public float cooldownTime = 0.2f;
    }
}