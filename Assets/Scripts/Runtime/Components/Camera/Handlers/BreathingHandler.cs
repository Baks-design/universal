using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public class BreathingHandler
    {
        readonly Transform transform;
        readonly CameraSettings settings;
        readonly CharacterController controller;
        readonly NoiseHandler noiseHandler;
        Vector3 finalRot;
        Vector3 finalPos;

        public BreathingHandler(Transform transform, CameraSettings settings, CharacterController controller)
        {
            this.transform = transform;
            this.settings = settings;
            this.controller = controller;

            noiseHandler = new NoiseHandler(settings);
        }

        public void UpdateBreathing()
        {
            if (controller.velocity.sqrMagnitude > 0f) return;

            noiseHandler.UpdateNoise();

            var noise = noiseHandler.Noise;

            switch (settings.transformTarget)
            {
                case TransformTarget.Position:
                    UpdatePosition(noise);
                    break;
                case TransformTarget.Rotation:
                    UpdateRotation(noise);
                    break;
                case TransformTarget.Both:
                    UpdatePosition(noise);
                    UpdateRotation(noise);
                    break;
            }
        }

        void UpdatePosition(Vector3 noise)
        {
            var localPos = transform.localPosition;

            finalPos.x = settings.x ? noise.x : localPos.x;
            finalPos.y = settings.y ? noise.y : localPos.y;
            finalPos.z = settings.z ? noise.z : localPos.z;

            transform.localPosition = finalPos;
        }

        void UpdateRotation(Vector3 noise)
        {
            var localEulers = transform.localRotation;

            finalRot.x = settings.x ? noise.x : localEulers.x;
            finalRot.y = settings.y ? noise.y : localEulers.y;
            finalRot.z = settings.z ? noise.z : localEulers.z;

            transform.localRotation = Quaternion.Euler(finalRot.x, finalRot.y, finalRot.z);
        }
    }
}