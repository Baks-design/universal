using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Sensitivity")]
        public Vector2 sensitivityAmount = new(1f, 1f);
        public float aimingSensitivityMultiplier = 0.5f;

        [Header("Smoothing")]
        public Vector2 smoothAmount = new(0.1f, 0.1f);
        public float maxRotationSpeed = 1000f;

        [Header("Clamping")]
        public Vector2 horizontalClamp = new(-90f, 90f);
        public Vector2 verticalClamp = new(-45, 45f);

        [Header("Recenter")]
        public float recenterSharpness = 3f;

        [Header("Zoom")]
        public float zoomFOV = 30f;
        public float zoomTransitionDuration = 0.3f;
        public float zoomSharpness = 4f;

        [Header("Movement Bob")]
        public float walkBobIntensity = 0.05f;
        public float runBobIntensity = 0.1f;
        public float walkBobFrequency = 3.5f;
        public float runBobFrequency = 4.5f;
        public float bobSmoothTime = 0.1f;
        public float bobReturnSmoothTime = 0.2f;
        public float sinMultiplier = 2f;
    }
}