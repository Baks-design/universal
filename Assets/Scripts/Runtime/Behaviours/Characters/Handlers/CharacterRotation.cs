using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly Transform character;
        readonly CharacterData data;
        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        const float ROTATION_ANGLE = 90f;
        const float ROTATION_OVERSHOOT = 0.0001f;
        const float COMPLETION_THRESHOLD = 0.999f;
        const float ANGLE_EPSILON = 0.1f;
        const float MIN_ROTATION_DURATION = 0.01f;
        float rotationProgress = 0f;
        bool isTurningInputPressed = false;

        public bool IsRotating { get; private set; } = false;

        public CharacterRotation(
            CharacterMovementController controller,
            Transform character,
            CharacterData data)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;

            targetRotation = startRotation = Quaternion.identity;
            rotationProgress = 0f;
            IsRotating = isTurningInputPressed = false;
        }

        public void HandleRotationRightInput() => HandleRotationInput(true);

        public void HandleRotationLeftInput() => HandleRotationInput(false);

        void HandleRotationInput(bool clockwise)
        {
            if (IsRotating) return;

            if (!isTurningInputPressed)
            {
                StartRotation(clockwise);
                isTurningInputPressed = true;
            }
            else
                isTurningInputPressed = false;
        }

        void StartRotation(bool clockwise)
        {
            startRotation = character.rotation;
            var rotationAmount = clockwise ?
                (ROTATION_ANGLE + ROTATION_OVERSHOOT) :
                -(ROTATION_ANGLE + ROTATION_OVERSHOOT);

            targetRotation = startRotation * Quaternion.Euler(0f, rotationAmount, 0f);
            rotationProgress = 0f;
            IsRotating = true;
            controller.CharacterMovement.IsMoving = true;

            // Immediate micro-rotation to start the interpolation
            character.rotation = Quaternion.Slerp(startRotation, targetRotation, 0.001f);
        }

        public void UpdateRotation()
        {
            if (!IsRotating) return;

            // Frame-rate independent smooth progression
            rotationProgress += Time.unscaledDeltaTime / Max(MIN_ROTATION_DURATION, data.rotateDuration);

            // Enhanced smoothing with curve evaluation
            var t = Clamp01(data.moveCurve.Evaluate(rotationProgress));
            character.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // Precision completion check
            var hadCompleted = rotationProgress >= COMPLETION_THRESHOLD;
            var isUnderAngle = Quaternion.Angle(character.rotation, targetRotation) < ANGLE_EPSILON;
            if (hadCompleted || isUnderAngle)
            {
                character.rotation = targetRotation;
                IsRotating = false;
                controller.CharacterMovement.IsMoving = false;
            }
        }
    }
}