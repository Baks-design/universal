using System;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [Serializable]
    public class CameraSettings
    {
        [Header("Sensitivity")]
        public Vector2 sensitivityAmount = new(1f, 1f);
        public float aimingSensitivityMultiplier = 0.5f;
        public float rotationSmoothness = 5f;

        [Header("Clamping")]
        public Vector2 verticalClamp = new(-45, 45f);

        [Header("Zoom")]
        public float zoomFOV = 30f;
        public float zoomTransitionDuration = 0.3f;
        public float zoomSharpness = 4f;

        [Header("Zoom")]
        public float swayAmount = 0f;
        public float swaySpeed = 0f;
        public float returnSpeed = 0f;
        public float changeDirectionMultiplier = 0f;
        public AnimationCurve swayCurve = new();
    }
}