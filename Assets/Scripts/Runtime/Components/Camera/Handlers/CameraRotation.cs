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
        float targetPitch;
        float targetYaw;
        float desiredTargetYaw;
        float desiredTargetPitch;

        public CameraRotation(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;
        }

        public void ResetPosition() => target.transform.localRotation = Quaternion.identity;

        public void HandleRotation() => ProcessRotation();

        void ProcessRotation()
        {
            var lookInput = PlayerMapInputProvider.Look.ReadValue<Vector2>();

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetPitch += lookInput.y * data.sensitivityAmount.y * Time.deltaTime;

            desiredTargetYaw = AMathfs.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = AMathfs.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            targetYaw = Mathf.Lerp(targetYaw, desiredTargetYaw, data.smoothAmount.x * Time.deltaTime);
            targetPitch = Mathf.Lerp(targetPitch, desiredTargetPitch, data.smoothAmount.y * Time.deltaTime);

            target.transform.localRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
        }
    }
}
