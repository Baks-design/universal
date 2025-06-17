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
        bool isRotatingBack;
        float rotationTimer;
        float desiredTargetYaw;
        float desiredTargetPitch;
        float targetYaw;
        float targetPitch;

        public CameraRotation(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;

            isRotatingBack = false;
            rotationTimer = 0f;
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
        }

        #region MOVEMENT
        public void ProcessRotation()
        {
            if (isRotatingBack) return;

            var lookInput = PlayerMapInputProvider.Look.ReadValue<Vector2>();

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

            desiredTargetYaw = AMathfs.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = AMathfs.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            targetYaw = Mathf.Lerp(targetYaw, desiredTargetYaw, data.smoothAmount.x * Time.deltaTime);
            targetPitch = Mathf.Lerp(targetPitch, desiredTargetPitch, data.smoothAmount.y * Time.deltaTime);

            currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            target.transform.localRotation = currentRotation;
        }
        #endregion

        #region RECENTERING
        public void ReturnToInitialRotation()
        {
            isRotatingBack = true;
            rotationTimer = 0f;
            ResetTargetToInitialValues();
        }

        public void UpdateRotateBackToInitial()
        {
            if (!isRotatingBack)
                return;

            rotationTimer += Time.deltaTime * data.recenterSpeed;
            var progress = Mathf.Clamp01(rotationTimer / data.recenterDuration);

            target.transform.localRotation =
                Quaternion.Slerp(
                    target.transform.localRotation,
                    initialRotation,
                    progress
                );

            if (rotationTimer >= data.recenterSpeed)
            {
                isRotatingBack = false;
                target.transform.localRotation = initialRotation;
                currentRotation = initialRotation;
            }
        }
        #endregion
    }
}
