using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }
    
    [CreateAssetMenu(menuName = "Data/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget;
        public float amplitude;
        public float frequency;
    }
}