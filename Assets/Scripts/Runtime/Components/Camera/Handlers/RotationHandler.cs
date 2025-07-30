using UnityEngine;
using Universal.Runtime.Components.Input;
using static Freya.Mathfs;

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
        readonly IInputDeviceServices deviceServices;
        Vector3 currentRotation;
        Vector3 rotationVelocity;

        public RotationHandler(
            CameraSettings settings, Transform transform, AimingHandler aimingHandler,
            IMovementInputReader movementInput, IInvestigateInputReader investigateInput,
            ICombatInputReader combatInput, IInputDeviceServices deviceServices)
        {
            this.settings = settings;
            this.transform = transform;
            this.aimingHandler = aimingHandler;
            this.movementInput = movementInput;
            this.investigateInput = investigateInput;
            this.combatInput = combatInput;
            this.deviceServices = deviceServices;

            currentRotation = transform.localRotation.eulerAngles;
        }

        public void UpdateRotation()
        {
            var input = GetProcessedInput();
            var targetRotation = CalculateTargetRotation(input);
            ApplySmoothedRotation(targetRotation);
            ApplyRotationToTransform();
        }

        Vector2 GetProcessedInput()
        {
            var rawInput = GetCombinedInput();

            if (deviceServices.IsGamepadLastActiveDevice)
                rawInput = ApplyResponseCurve(rawInput);

            var sensitivity = deviceServices.IsGamepadLastActiveDevice ?
                settings.gamepadSensitivity :
                settings.mouseSensitivity;

            return rawInput * sensitivity;
        }

        Vector2 GetCombinedInput() => movementInput.LookDirection + investigateInput.LookDirection + combatInput.LookDirection;

        Vector2 ApplyResponseCurve(Vector2 input) => input.normalized * settings.gamepadResponseCurve.Evaluate(input.magnitude);

        Vector3 CalculateTargetRotation(Vector2 input)
        {
            var targetRotation = currentRotation + new Vector3(-input.y, input.x, 0f);
            targetRotation.x = Clamp(targetRotation.x, settings.lookAngleMinMax.x, settings.lookAngleMinMax.y);
            return targetRotation;
        }

        void ApplySmoothedRotation(Vector3 targetRotation)
        => currentRotation = Vector3.SmoothDamp(
            currentRotation, targetRotation,
            ref rotationVelocity,
            GetCurrentSmoothTime(),
            Infinity,
            Time.deltaTime);

        float GetCurrentSmoothTime()
        {
            var baseSmoothTime = settings.rotationSmoothTime;
            return aimingHandler.IsZooming
                ? baseSmoothTime * settings.aimingSensitivityMultiplier
                : baseSmoothTime;
        }

        void ApplyRotationToTransform() => transform.localRotation = Quaternion.Euler(currentRotation);
    }
}