using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation //FIXME: ROTATION ON START BUG GAME
    {
        readonly CharacterMovementController controller;
        readonly Transform character;
        readonly CharacterData data;
        Quaternion startRotation;
        Quaternion targetRotation;
        bool isTurningInputPressed;
        float rotationProgress;
        float lastInputTime;

        public bool IsRotating { get; private set; }

        public CharacterRotation(
            CharacterMovementController controller,
            Transform character,
            CharacterData data)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;

            startRotation = Quaternion.identity;
            targetRotation = character.transform.rotation;
            rotationProgress = 0f;
            isTurningInputPressed = false;
            lastInputTime = 0f;
            IsRotating = false;
        }

        public void UpdateRotation()
        {
            if (!IsRotating)
                return;

            rotationProgress += Time.deltaTime / data.rotateDuration;
            var t = data.moveCurve.Evaluate(rotationProgress);
            character.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            if (rotationProgress >= 1f)
            {
                character.rotation = targetRotation;
                IsRotating = false;
                controller.CharacterMovement.IsMoving = false;
            }
        }

        public void HandleRotationInput()
        {
            if (controller.CharacterMovement.IsMoving ||
                IsRotating ||
                Time.time < lastInputTime + data.inputCooldown)
                return;

            var rotateInput = PlayerMapInputProvider.Turn.ReadValue<float>();
            if (Mathf.Abs(rotateInput) > 0.1f)
            {
                if (!isTurningInputPressed)
                {
                    StartRotation(rotateInput > 0f);
                    lastInputTime = Time.time;
                    isTurningInputPressed = true;
                }
            }
            else
                isTurningInputPressed = false;
        }

        void StartRotation(bool clockwise)
        {
            startRotation = character.rotation;
            targetRotation = startRotation * Quaternion.Euler(0f, clockwise ? 90f : -90f, 0f);
            rotationProgress = 0f;
            IsRotating = true;
            controller.CharacterMovement.IsMoving = true;
        }

        public void ForceSnapRotation()
        {
            character.rotation = targetRotation;
            IsRotating = false;
            controller.CharacterMovement.IsMoving = false;
        }
    }
}