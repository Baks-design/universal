using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Components.Camera
{
    public class RecoilHandler
    {
        readonly CameraSettings settings;
        readonly Transform camera;
        Quaternion currentRotation;
        Vector3 targetRotation;
        bool isAiming;
        public Vector3 recoilRotationHipfire;
        public Vector3 recoilRotationAiming;

        public RecoilHandler(CameraSettings settings, Transform camera)
        {
            this.settings = settings;
            this.camera = camera;
        }

        public void UpdateRecoil()
        {
            targetRotation = Helpers.ExpDecay(
                targetRotation, Vector3.zero, settings.recoilReturnSpeed * Time.deltaTime);

            currentRotation = Helpers.ExpDecayRotation(
                Quaternion.Euler(0f, currentRotation.y, 0f),
                Quaternion.Euler(0f, targetRotation.y, 0f),
                settings.recoilRotationSpeed * Time.deltaTime);

            camera.localRotation = currentRotation;
        }

        public void AddRecoil() => targetRotation += isAiming ? settings.recoilRotationAiming : settings.recoilRotationHipfire;

        public void SetAiming(bool aiming) => isAiming = aiming;
    }
}