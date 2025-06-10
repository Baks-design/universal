using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Camera Movement Settings")]
        [Min(30f)] public float topClamp = 70f;
        [Min(0f)] public float bottomClamp = -30f;
        [Min(1f)] public float smoothAmount = 40f;
    }
}