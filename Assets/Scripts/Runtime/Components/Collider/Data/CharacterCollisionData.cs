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
    }
}