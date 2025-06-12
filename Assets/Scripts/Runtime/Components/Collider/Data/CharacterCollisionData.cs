using UnityEngine;

namespace Universal.Runtime.Components.Collider
{
    [CreateAssetMenu(menuName = "Data/Character/Collider")]
    public class CharacterCollisionData : ScriptableObject
    {
        [Header("Floor Check")]
        public LayerMask floorCollisionLayers = ~0;
        public float floorRayOriginPositionOffset = -0.25f;
        public float floorRayMaxDistance = 0.1f;

        [Header("Roof Check")]
        public LayerMask roofCollisionLayers = ~0;
        public float roofOriginPositionOffset = -0.25f;
        public float roofRayMaxDistance = 0.1f;

        [Header("Obstacle Check")]
        public LayerMask obstacleCollisionLayers = ~0;
        public float obstacleRayOriginPositionOffset = 0.4f;
        public float obstacleRayRadius = 0.2f;
        public float obstacleRayMaxDistance = 0.4f;

        [Header("Interaction Check")]
        public LayerMask interactableCollisionLayers = ~0;
        public float interactionRayRadius = 0.3f;
        public float interactionMaxRayDistance = 0.3f;

        [Header("Push")]
        public LayerMask pushCollsionLayers;
        public bool canPush = true;
        [Range(0.5f, 5f)] public float strength = 5f;
    }
}