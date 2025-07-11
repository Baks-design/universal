using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Movement Settings")]
        public Vector2 verticalClamp = new(-45f, 45f);
        public Vector2 horizontalClamp = new(-90, 90f);
        public Vector2 sensitivityAmount = new(10f, 10f);
        public Vector2 smoothAmount = new(10f, 10f);

        [Header("Recenter Settings")]
        [Min(0.1f)] public float recenterDuration = 1f;

        [Header("Aiming Settings")]
        public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Range(20f, 60f)] public float zoomFOV = 45f;
        [Range(0.1f, 1f)] public float zoomTransitionDuration = 0.3f;
    }
}