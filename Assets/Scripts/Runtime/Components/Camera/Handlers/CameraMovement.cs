using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraMovement
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly IPlayerInputReader inputReader;
        Quaternion currentRotation;
        float desiredTargetYaw, desiredTargetPitch, targetYaw, targetPitch;
        float yawSmoothVelocity, pitchSmoothVelocity;

        public Quaternion CurrentRotation => currentRotation;

        public CameraMovement(
            CameraData data,
            CinemachineCamera target,
            IPlayerInputReader inputReader,
            Quaternion initialRotation)
        {
            this.data = data;
            this.target = target;
            this.inputReader = inputReader;
            currentRotation = initialRotation;

            ResetTargetToInitialValues(initialRotation);
        }

        public void ResetTargetToInitialValues(Quaternion initialRotation)
        {
            desiredTargetYaw = Helpers.NormalizeAngle(initialRotation.eulerAngles.y);
            desiredTargetPitch = Helpers.NormalizeAngle(initialRotation.eulerAngles.x);
            targetYaw = desiredTargetYaw;
            targetPitch = desiredTargetPitch;
            yawSmoothVelocity = pitchSmoothVelocity = 0f;
        }

        public void ProcessRotation()
        {
            var lookInput = inputReader.LookDirection;

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch -= lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

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

        public void SetCurrentRotation(Quaternion rotation)
        {
            currentRotation = rotation;
            target.transform.localRotation = rotation;
        }
    }
}