using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation //TODO: Testing with MathemathicsX
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        Quaternion initialRotation;
        Quaternion currentRotation;
        float rotationTimer, desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;
        float yawSmoothVelocity, pitchSmoothVelocity;
        const float SMOOTHTARGETEPSILON = 0.001f;

        public bool IsRotatingBack { get; set; }

        public CameraRotation(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;

            IsRotatingBack = false;
            rotationTimer = yawSmoothVelocity = pitchSmoothVelocity = 0f;
            initialRotation = target.transform.localRotation;
            currentRotation = initialRotation;
            ResetTargetToInitialValues();
        }

        void ResetTargetToInitialValues()
        {
            desiredTargetYaw = initialRotation.eulerAngles.y;
            desiredTargetPitch = initialRotation.eulerAngles.x;
            targetYaw = desiredTargetYaw;
            targetPitch = desiredTargetPitch;
            yawSmoothVelocity = pitchSmoothVelocity = 0f;
        }

        #region MOVEMENT
        public void ProcessRotation()
        {
            if (IsRotatingBack) return;

            var lookInput = PlayerMapInputProvider.Look.ReadValue<Vector2>();

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

            desiredTargetYaw = Mathf.Clamp(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = Mathf.Clamp(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            // Use SmoothDamp for more consistent smoothing across framerates
            targetYaw = Mathf.SmoothDamp(
                targetYaw, desiredTargetYaw, ref yawSmoothVelocity,
                1f / data.smoothAmount.x, Mathf.Infinity, Time.deltaTime);
            targetPitch = Mathf.SmoothDamp(
                targetPitch, desiredTargetPitch, ref pitchSmoothVelocity,
                1f / data.smoothAmount.y, Mathf.Infinity, Time.deltaTime);

            // Skip rotation update if changes are negligible
            if (Quaternion.Angle(currentRotation, Quaternion.Euler(targetPitch, targetYaw, 0f)) > SMOOTHTARGETEPSILON)
            {
                currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
                target.transform.localRotation = currentRotation;
            }
        }
        #endregion

        #region RECENTERING
        public void ReturnToInitialRotation()
        {
            IsRotatingBack = true;
            rotationTimer = 0f;
            ResetTargetToInitialValues();
        }

        public void UpdateRotateBackToInitial()
        {
            if (!IsRotatingBack) return;

            rotationTimer += Time.deltaTime;
            var progress = Mathf.Clamp01(rotationTimer / data.recenterDuration);

            // Use spherical interpolation with eased progress for smoother return
            var easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation,
                initialRotation,
                easedProgress
            );

            if (progress >= 1f - SMOOTHTARGETEPSILON)
            {
                IsRotatingBack = false;
                target.transform.localRotation = initialRotation;
                currentRotation = initialRotation;
            }
        }
        #endregion
    }
}