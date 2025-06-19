using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        Quaternion initialRotation;
        Quaternion currentRotation;
        float rotationTimer, desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;
        float yawSmoothVelocity, pitchSmoothVelocity;
        const float SMOOTH_TARGET_EPSILON = 0.001f;

        public bool IsRotatingBack { get; private set; }

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

            desiredTargetYaw = AMathfs.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = AMathfs.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            // Use SmoothDamp for more consistent smoothing across framerates
            targetYaw = Mathf.SmoothDamp(
                targetYaw, desiredTargetYaw, ref yawSmoothVelocity,
                1f / data.smoothAmount.x, Mathf.Infinity, Time.deltaTime);
            targetPitch = Mathf.SmoothDamp(
                targetPitch, desiredTargetPitch, ref pitchSmoothVelocity,
                1f / data.smoothAmount.y, Mathf.Infinity, Time.deltaTime);

            // Skip rotation update if changes are negligible
            if (Quaternion.Angle(currentRotation, Quaternion.Euler(targetPitch, targetYaw, 0f)) > SMOOTH_TARGET_EPSILON)
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
            if (!IsRotatingBack)
                return;

            rotationTimer += Time.deltaTime;
            var progress = Mathf.Clamp01(rotationTimer / data.recenterDuration);

            // Use spherical interpolation with eased progress for smoother return
            var easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation,
                initialRotation,
                easedProgress
            );

            if (progress >= 1f - SMOOTH_TARGET_EPSILON)
            {
                IsRotatingBack = false;
                target.transform.localRotation = initialRotation;
                currentRotation = initialRotation;
            }
        }
        #endregion
    }























    // public class CameraRotation
    // {
    //     readonly CameraData data;
    //     readonly CinemachineCamera target;
    //     Quaternion initialRotation;
    //     Quaternion currentRotation;
    //     float rotationTimer, desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;

    //     public bool IsRotatingBack { get; private set; }

    //     public CameraRotation(CameraData data, CinemachineCamera target)
    //     {
    //         this.data = data;
    //         this.target = target;

    //         IsRotatingBack = false;
    //         rotationTimer = 0f;
    //         initialRotation = target.transform.localRotation;
    //         currentRotation = initialRotation;
    //         ResetTargetToInitialValues();
    //     }

    //     void ResetTargetToInitialValues()
    //     {
    //         desiredTargetYaw = initialRotation.eulerAngles.y;
    //         desiredTargetPitch = initialRotation.eulerAngles.x;
    //         targetYaw = desiredTargetYaw;
    //         targetPitch = desiredTargetPitch;
    //     }

    //     #region MOVEMENT
    //     public void ProcessRotation()
    //     {
    //         if (IsRotatingBack) return;

    //         var lookInput = PlayerMapInputProvider.Look.ReadValue<Vector2>();

    //         desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
    //         desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

    //         desiredTargetYaw = AMathfs.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
    //         desiredTargetPitch = AMathfs.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

    //         targetYaw = Mathf.Lerp(targetYaw, desiredTargetYaw, data.smoothAmount.x * Time.deltaTime);
    //         targetPitch = Mathf.Lerp(targetPitch, desiredTargetPitch, data.smoothAmount.y * Time.deltaTime);

    //         currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
    //         target.transform.localRotation = currentRotation;
    //     }
    //     #endregion

    //     #region RECENTERING
    //     public void ReturnToInitialRotation()
    //     {
    //         IsRotatingBack = true;
    //         rotationTimer = 0f;
    //         ResetTargetToInitialValues();
    //     }

    //     public void UpdateRotateBackToInitial()
    //     {
    //         if (!IsRotatingBack)
    //             return;

    //         rotationTimer += Time.deltaTime * data.recenterSpeed;
    //         var progress = Mathf.Clamp01(rotationTimer / data.recenterDuration);

    //         target.transform.localRotation =
    //             Quaternion.Slerp(
    //                 target.transform.localRotation,
    //                 initialRotation,
    //                 progress
    //             );

    //         if (rotationTimer >= data.recenterSpeed)
    //         {
    //             IsRotatingBack = false;
    //             target.transform.localRotation = initialRotation;
    //             currentRotation = initialRotation;
    //         }
    //     }
    //     #endregion
    // }
}
