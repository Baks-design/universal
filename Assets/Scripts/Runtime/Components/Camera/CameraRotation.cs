using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly Transform yawTransform;
        readonly Transform pitchTransform;
        float yaw;
        float pitch;
        float desiredPitch;
        float desiredYaw;

        public CameraRotation(CameraData data, Transform yawTransform, Transform pitchTransform)
        {
            this.data = data;
            this.yawTransform = yawTransform;
            this.pitchTransform = pitchTransform;

            desiredYaw = yaw = yawTransform.eulerAngles.y;
        }

        public void HandleRotation(float deltaTime)
        {
            CalculateRotation(deltaTime);
            ApplySmoothRotation(deltaTime);
            ApplyRotation();
        }

        void CalculateRotation(float deltaTime)
        {
            var lookInput = PlayerMapInputProvider.Look;
            desiredYaw += lookInput.x * deltaTime * data.smoothAmount;
            desiredPitch += lookInput.y * deltaTime * data.smoothAmount;
            desiredPitch = Mathf.Clamp(desiredPitch, data.bottomClamp, data.topClamp);
        }

        void ApplySmoothRotation(float deltaTime)
        {
            var smoothFactor = data.smoothAmount * deltaTime;
            yaw = Mathf.Lerp(yaw, desiredYaw, smoothFactor);
            pitch = Mathf.Lerp(pitch, desiredPitch, smoothFactor);
        }

        void ApplyRotation()
        {
            yawTransform.eulerAngles = new Vector3(0f, yaw, 0f);
            pitchTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }
}
