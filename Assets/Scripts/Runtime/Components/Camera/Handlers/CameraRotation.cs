using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly IPlayerInputReader inputReader;
        Quaternion initialRotation;
        Quaternion currentRotation;
        float rotationTimer, desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;
        float yawSmoothVelocity, pitchSmoothVelocity;
        const float SMOOTHTARGETEPSILON = 0.001f;

        public bool IsRotatingBack { get; set; }

        public CameraRotation(
            CameraData data,
            CinemachineCamera target,
            IPlayerInputReader inputReader)
        {
            this.data = data;
            this.target = target;
            this.inputReader = inputReader;

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

            var lookInput = inputReader.LookDirection;

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

            desiredTargetYaw = Clamp(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = Clamp(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            // Use SmoothDamp for more consistent smoothing across framerates
            targetYaw = SmoothDamp(
                targetYaw, desiredTargetYaw, ref yawSmoothVelocity,
                1f / data.smoothAmount.x, Infinity, Time.deltaTime);
            targetPitch = SmoothDamp(
                targetPitch, desiredTargetPitch, ref pitchSmoothVelocity,
                1f / data.smoothAmount.y, Infinity, Time.deltaTime);

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
            var progress = Clamp01(rotationTimer / data.recenterDuration);

            var easedProgress = Smooth01(progress);
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