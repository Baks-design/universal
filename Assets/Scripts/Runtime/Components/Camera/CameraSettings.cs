using System;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }

    [Serializable]
    public class CameraSettings
    {
        [Header("Input Scaling")]
        public float mouseSensitivity = 1f;
        public float gamepadSensitivity = 2f;
        public AnimationCurve gamepadResponseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Rotation Settings")]
        public Vector2 lookAngleMinMax = new(-45, 45f);

        [Header("Smoothing")]
        public float rotationSmoothTime = 0.1f;
        public float aimingSensitivityMultiplier = 0.5f;

        [Header("Zoom Settings")]
        public float zoomFOV = 40f;
        public float zoomTransitionDuration = 0.25f;
        public AnimationCurve zoomCurve = new();

        [Header("Run Settings")]
        public float runFOV = 70f;
        public float runTransitionDuration = 0.75f;
        public float runReturnTransitionDuration = 0.5f;
        public AnimationCurve runCurve = new();

        [Header("Sway Settings")]
        public float swayAmount = 1f;
        public float swaySpeed = 1f;
        public float returnSpeed = 3f;
        public float changeDirectionMultiplier = 4f;
        public AnimationCurve swayCurve = new();

        [Header("Breath Settings")]
        public TransformTarget transformTarget = TransformTarget.Rotation;
        public float amplitude = 1f;
        public float frequency = 0.5f;
        public bool x = true;
        public bool y = true;
        public bool z = false;

        [Header("Recoil Settings")]
        public float recoilRotationSpeed = 6f;
        public float recoilReturnSpeed = 25f;
        public Vector3 recoilRotationHipfire = new(2f, 0.5f, 0.25f);
        public Vector3 recoilRotationAiming = new(1.2f, 0.3f, 0.15f);
    }
}