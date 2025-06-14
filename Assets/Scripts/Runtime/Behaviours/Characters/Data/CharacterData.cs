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

        [Header("Movement")]
        [Range(0.1f, 1f)] public float inputCooldown = 0.2f;
        [Range(0.1f, 1f)] public float inputDeadzone = 0.2f;
        [Range(0.1f, 1f)] public float diagonalThreshold = 0.1f;
        public LayerMask obstacleMask;
        [Range(0.1f, 1f)] public float obstacleCheckRadius = 0.4f;
        [Range(0.1f, 1f)] public float moveDuration = 0.3f;
        [AnimationCurveSettings] public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Range(0.1f, 1f)] public float rotateDuration = 0.2f;

        [Header("Footsteps")]
        [Range(0.5f, 1f)] public float walkStepInterval = 0.5f;
        [Range(0.3f, 1f)] public float runStepInterval = 0.3f;
    }
}