using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Camera Movement Settings")]
        public float topVerticalClamp = 45f;
        public float bottomVerticalClamp = -45f;
        public float topHorizontalClamp = 45f;
        public float bottomHorizontalClamp = -45f;
        public float smoothAmount = 10f;
    }
}