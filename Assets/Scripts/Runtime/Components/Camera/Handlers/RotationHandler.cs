using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Components.Camera
{
    public class RotationHandler
    {
        readonly CameraSettings settings;
        readonly Transform transform;
        readonly AimingHandler aimingHandler;
        readonly IMovementInputReader movementInput;
        readonly IInvestigateInputReader investigateInput;
        readonly ICombatInputReader combatInput;
        float currentYaw;
        float currentPitch;
        float targetYaw;
        float targetPitch;

        public RotationHandler(
            CameraSettings settings, Transform transform, AimingHandler aimingHandler,
            IMovementInputReader movementInput, IInvestigateInputReader investigateInput,
            ICombatInputReader combatInput)
        {
            this.settings = settings;
            this.transform = transform;
            this.aimingHandler = aimingHandler;
            this.movementInput = movementInput;
            this.investigateInput = investigateInput;
            this.combatInput = combatInput;
        }

        public void SetRotationImmediately(Vector2 rotation)
        {
            targetYaw = currentYaw = rotation.x;
            targetPitch = currentPitch = Helpers.ClampAngle(
                rotation.y, settings.verticalClamp.x, settings.verticalClamp.y);
            UpdateRotationHandler();
        }

        public void UpdateRotation()
        {
            CalculateDesiredRotation();
            ApplySmoothing();
            UpdateRotationHandler();
        }

        void CalculateDesiredRotation()
        {
            var lookInput = movementInput.LookDirection + investigateInput.LookDirection + combatInput.LookDirection;

            var sensitivityMultiplier = aimingHandler.IsZooming ? settings.aimingSensitivityMultiplier : 1f;

            targetYaw += lookInput.x * settings.sensitivityAmount.x * Time.deltaTime * sensitivityMultiplier;
            targetPitch -= lookInput.y * settings.sensitivityAmount.y * Time.deltaTime * sensitivityMultiplier;

            targetPitch = Helpers.ClampAngle(targetPitch, settings.verticalClamp.x, settings.verticalClamp.y);
        }

        void ApplySmoothing()
        {
            var dt = Time.deltaTime * settings.rotationSmoothness;
            currentYaw = Helpers.ExpDecay(currentYaw, targetYaw, dt);
            currentPitch = Helpers.ExpDecay(currentPitch, targetPitch, dt);
        }

        void UpdateRotationHandler() => transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}
//FIXME : Adjust input on new input system