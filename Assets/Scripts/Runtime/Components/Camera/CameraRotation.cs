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
        float cinemachineTargetPitch;
        float cinemachineTargetYaw;
        float desiredTargetYaw;
        float desiredTargetPitch;

        public CameraRotation(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;
        }

        public void HandleRotation()
        {
            CalculateRotation();
            ApplySmoothRotation();
            ApplyRotation();
        }

        void CalculateRotation()
        {
            var lookInput = PlayerMapInputProvider.Look.ReadValue<Vector2>();

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

            desiredTargetYaw = GameHelper.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = GameHelper.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);
        }

        void ApplySmoothRotation()
        {
            cinemachineTargetYaw = Mathf.Lerp(cinemachineTargetYaw, desiredTargetYaw, data.smoothAmount.x * Time.deltaTime);
            cinemachineTargetPitch = Mathf.Lerp(cinemachineTargetPitch, desiredTargetPitch, data.smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation() => target.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f);
    }
}
