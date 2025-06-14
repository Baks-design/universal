using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly CharacterController character;
        readonly CharacterData data;
        Quaternion targetRotation;
        Quaternion startRotation;
        float rotationProgress;
        float lastInputTime;
        bool isTurningInputPressed;

        public bool IsRotating { get; private set; }

        public CharacterRotation(
            CharacterMovementController controller,
            CharacterController character,
            CharacterData data)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;

            IsRotating = false;
            isTurningInputPressed = false;
            targetRotation = character.transform.rotation;
        }

        public void UpdateRotation()
        {
            if (IsRotating)
            {
                rotationProgress += Time.deltaTime / data.rotateDuration;
                var t = data.moveCurve.Evaluate(rotationProgress);
                character.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

                if (rotationProgress >= 1f)
                {
                    character.transform.rotation = targetRotation;
                    IsRotating = false;
                    controller.CharacterMovement.IsMoving = false;
                }
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
            startRotation = character.transform.rotation;
            targetRotation = startRotation * Quaternion.Euler(0, clockwise ? 90f : -90f, 0f);
            rotationProgress = 0f;
            IsRotating = true;
            controller.CharacterMovement.IsMoving = true;
        }

        public void ForceSnapRotation()
        {
            character.transform.rotation = targetRotation;
            IsRotating = false;
            controller.CharacterMovement.IsMoving = false;
        }
    }
}