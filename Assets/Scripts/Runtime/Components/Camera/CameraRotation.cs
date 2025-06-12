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
        Vector2 lookInput;
        float cinemachineTargetPitch;
        float cinemachineTargetYaw;
        float smoothFactor;
        float desiredTargetYaw;
        float desiredTargetPitch;
        readonly bool IsCurrentDeviceMouse = true;

        public CameraRotation(CameraData data, CinemachineCamera target, Vector2 lookInput)
        {
            this.data = data;
            this.target = target;
            this.lookInput = lookInput;

            cinemachineTargetPitch = target.transform.localRotation.eulerAngles.x;
            cinemachineTargetYaw = target.transform.localRotation.eulerAngles.y;
        }

        public void HandleRotation()
        {
            CalculateSmoothFactor();
            CalculateRotation();
            ApplySmoothRotation();
            ApplyRotation();
        }

        void CalculateSmoothFactor()
        {
            var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1f : Time.deltaTime;
            smoothFactor = data.smoothAmount * deltaTimeMultiplier;
        }

        void CalculateRotation()
        {
            desiredTargetYaw += lookInput.x * smoothFactor;
            desiredTargetPitch += lookInput.y * smoothFactor;

            desiredTargetYaw = GameHelper.ClampAngle(cinemachineTargetYaw, data.bottomHorizontalClamp, data.topHorizontalClamp);
            desiredTargetPitch = GameHelper.ClampAngle(cinemachineTargetPitch, data.bottomVerticalClamp, data.topVerticalClamp);
        }

        void ApplySmoothRotation()
        {
            cinemachineTargetYaw = Mathf.Lerp(cinemachineTargetYaw, desiredTargetYaw, smoothFactor);
            cinemachineTargetPitch = Mathf.Lerp(cinemachineTargetPitch, desiredTargetPitch, smoothFactor);
        }

        void ApplyRotation() => target.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f);
    }
}
