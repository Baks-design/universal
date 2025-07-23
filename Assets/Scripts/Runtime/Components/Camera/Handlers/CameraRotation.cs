using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraSettings settings;
        readonly Transform transform;
        readonly CameraAiming aim;
        const float FrameRateNormalization = 60f;
        float currentYaw;
        float currentPitch;
        float targetYaw;
        float targetPitch;

        public CameraRotation(CameraSettings settings, Transform transform, CameraAiming aim)
        {
            this.settings = settings;
            this.transform = transform;
            this.aim = aim;
        }

        public void UpdateRotation(Vector2 lookInput)
        {
            CalculateDesiredRotation(lookInput);
            ApplySmoothing();
            UpdateCameraRotation();
        }

        void CalculateDesiredRotation(Vector2 lookInput)
        {
            var frameIndependentSensitivity = Time.deltaTime * FrameRateNormalization;
            var sensitivityMultiplier = aim.IsAiming
                ? settings.aimingSensitivityMultiplier
                : 1f;

            targetYaw += lookInput.x * settings.sensitivityAmount.x * frameIndependentSensitivity * sensitivityMultiplier;
            targetPitch -= lookInput.y * settings.sensitivityAmount.y * frameIndependentSensitivity * sensitivityMultiplier;

            targetPitch = Helpers.ClampAngle(targetPitch, settings.verticalClamp.x, settings.verticalClamp.y);
        }

        void ApplySmoothing()
        {
            var smoothFactor = Time.deltaTime * settings.rotationSmoothness;
            currentYaw = Helpers.ExpDecay(currentYaw, targetYaw, smoothFactor);
            currentPitch = Helpers.ExpDecay(currentPitch, targetPitch, smoothFactor);
        }

        public void SetRotationImmediately(Vector2 rotation)
        {
            targetYaw = currentYaw = rotation.x;
            targetPitch = currentPitch = Helpers.ClampAngle(rotation.y, settings.verticalClamp.x, settings.verticalClamp.y);
            UpdateCameraRotation();
        }

        void UpdateCameraRotation() => transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}