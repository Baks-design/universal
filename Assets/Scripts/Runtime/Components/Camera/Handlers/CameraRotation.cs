using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly IPlayerInputReader inputReader;
        Quaternion initialRotation,currentRotation, recenteringStartRotation; 
        float rotationTimer, desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;
        float yawSmoothVelocity, pitchSmoothVelocity, recenteringAngularDistance; 
        const float RECENTER_COMPLETE_THRESHOLD = 0.999f; 

        public bool IsRotatingBack { get; private set; } 

        public CameraRotation(
            CameraData data,
            CinemachineCamera target,
            IPlayerInputReader inputReader)
        {
            this.data = data;
            this.target = target;
            this.inputReader = inputReader;

            ResetToInitialRotation();
        }

        void ResetToInitialRotation()
        {
            IsRotatingBack = false;
            rotationTimer = 0f;
            initialRotation = target.transform.localRotation;
            currentRotation = initialRotation;
            ResetTargetToInitialValues();
        }

        void ResetTargetToInitialValues()
        {
            desiredTargetYaw = Helpers.NormalizeAngle(initialRotation.eulerAngles.y);
            desiredTargetPitch = Helpers.NormalizeAngle(initialRotation.eulerAngles.x);
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

            desiredTargetYaw = Helpers.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = Helpers.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            targetYaw = SmoothDamp(
                targetYaw, desiredTargetYaw, ref yawSmoothVelocity,
                1f / data.smoothAmount.x, Infinity, Time.deltaTime);
            targetPitch = SmoothDamp(
                targetPitch, desiredTargetPitch, ref pitchSmoothVelocity,
                1f / data.smoothAmount.y, Infinity, Time.deltaTime);

            currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            target.transform.localRotation = currentRotation;
        }
        #endregion

        #region RECENTERING
        public void ReturnToInitialRotation()
        {
            if (IsRotatingBack) return;

            IsRotatingBack = true;
            rotationTimer = 0f;
            recenteringStartRotation = currentRotation;

            // Calculate angular distance (degrees) between current and initial rotation
            recenteringAngularDistance = Quaternion.Angle(currentRotation, initialRotation);

            yawSmoothVelocity = pitchSmoothVelocity = 0f;
        }

        public void UpdateRotateBackToInitial()
        {
            if (!IsRotatingBack) return;

            // Dynamic duration: Base duration scales with angular distance
            var dynamicDuration = data.recenterDuration * (recenteringAngularDistance / 180f);

            rotationTimer += Time.deltaTime;
            var progress = Clamp01(rotationTimer / dynamicDuration);
            var easedProgress = Smooth01(progress);

            currentRotation = Quaternion.Slerp(
                recenteringStartRotation,
                initialRotation,
                easedProgress
            );
            target.transform.localRotation = currentRotation;

            // Update target values
            var euler = currentRotation.eulerAngles;
            targetYaw = Helpers.NormalizeAngle(euler.y);
            targetPitch = Helpers.NormalizeAngle(euler.x);
            desiredTargetYaw = targetYaw;
            desiredTargetPitch = targetPitch;

            if (progress >= RECENTER_COMPLETE_THRESHOLD)
                CompleteRecentering();
        }

        void CompleteRecentering()
        {
            currentRotation = initialRotation;
            target.transform.localRotation = initialRotation;
            ResetTargetToInitialValues();
            IsRotatingBack = false;
        }
        #endregion
    }
}