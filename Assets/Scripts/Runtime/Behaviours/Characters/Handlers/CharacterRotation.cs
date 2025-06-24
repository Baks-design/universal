using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly Transform character;
        readonly CharacterData data;
        readonly IPlayerInputReader inputReader;
        Quaternion startRotation, targetRotation;
        bool isTurningInputPressed;
        float rotationProgress, lastInputTime, inputBufferTimer;
        const float INPUT_BUFFER_TIME = 0.1f;

        public bool IsRotating { get; private set; }

        public CharacterRotation(
            CharacterMovementController controller,
            Transform character,
            CharacterData data,
            IPlayerInputReader inputReader)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;
            this.inputReader = inputReader;

            targetRotation = startRotation = Quaternion.identity;
            lastInputTime = rotationProgress = inputBufferTimer = 0f;
            IsRotating = isTurningInputPressed = false;
        }

        public void UpdateRotation()
        {
            if (!IsRotating) return;

            // Frame-rate independent smooth progression
            rotationProgress += Time.unscaledDeltaTime / Mathf.Max(0.01f, data.rotateDuration);

            // Enhanced smoothing with curve evaluation
            var t = Mathf.Clamp01(data.moveCurve.Evaluate(rotationProgress));
            character.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // Precision completion check
            if (rotationProgress >= 0.999f || Quaternion.Angle(character.rotation, targetRotation) < 0.1f)
            {
                character.rotation = targetRotation;
                IsRotating = false;
                controller.CharacterMovement.IsMoving = false;
            }
        }

        public void HandleRotationInput()
        {
            // Early exit conditions
            if (controller.CharacterMovement.IsMoving ||
                IsRotating ||
                Time.time < lastInputTime + data.inputCooldown)
                return;

            // Process input buffer
            if (inputBufferTimer > 0f)
                inputBufferTimer -= Time.unscaledDeltaTime;

            var rotateInput = inputReader.Turning;
            var absInput = Mathf.Abs(rotateInput);
            // Input with deadzone and buffer consideration
            if (absInput > 0.1f)
            {
                if (!isTurningInputPressed || inputBufferTimer > 0f)
                {
                    StartRotation(rotateInput > 0f);
                    lastInputTime = Time.time;
                    isTurningInputPressed = true;
                    inputBufferTimer = 0f; // Clear buffer after use
                }
            }
            else
            {
                isTurningInputPressed = false;
                // Store input in buffer if rotation was blocked
                if (absInput > 0.1f && (IsRotating || controller.CharacterMovement.IsMoving))
                    inputBufferTimer = INPUT_BUFFER_TIME;
            }
        }

        void StartRotation(bool clockwise)
        {
            startRotation = character.rotation;
            // Precise target rotation with slight overshoot to ensure completion
            targetRotation = startRotation * Quaternion.Euler(0f, clockwise ? 90.0001f : -90.0001f, 0f);
            rotationProgress = 0f;
            IsRotating = true;
            controller.CharacterMovement.IsMoving = true;

            // Immediate micro-rotation to start the interpolation
            character.localRotation = Quaternion.Slerp(startRotation, targetRotation, 0.001f);
        }
    }
}