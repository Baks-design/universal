using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class PhysicsSettings
    {
        [Header("Ground Settings")]
        public LayerMask groundLayer = ~0;
        [Range(0f, 1f)] public float rayLength = 0.1f;
        [Range(0.01f, 1f)] public float raySphereRadius = 0.1f;

        [Header("Obstacles Settings")]
        public LayerMask obstacleLayers = ~0;
        [Range(0f, 1f)] public float rayObstacleLength = 0.1f;
        [Range(0.01f, 1f)] public float rayObstacleSphereRadius = 0.1f;
    }
}