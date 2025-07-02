using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterHeadBobbing
    {
        readonly CharacterMovementController movementController;
        readonly Transform headTransform;
        readonly CharacterData data;
        readonly float defaultYPos;
        bool wasMoving;
        float timer;
        float currentBobAmount;
        float currentBobSpeed;
        float targetBobAmount;
        float targetBobSpeed;

        public float CurrentYPos { get; private set; }

        public CharacterHeadBobbing(
            CharacterMovementController movementController,
            CharacterData data,
            Transform headTransform)
        {
            this.movementController = movementController;
            this.data = data;
            this.headTransform = headTransform;

            defaultYPos = headTransform.localPosition.y;
            currentBobAmount = data.walkBobAmount;
            currentBobSpeed = data.walkBobSpeed;
        }

        public void HandleMovementBob()
        {
            // Only bob when actually moving (not during rotation)
            if (movementController.CharacterMovement.IsMoving &&
                !movementController.CharacterRotation.IsRotating)
            {
                // Progress the bob timer
                timer += Time.deltaTime * currentBobSpeed;

                // Smoothly transition between walk/run bob
                targetBobAmount = movementController.CharacterMovement.IsRunning ? data.runBobAmount : data.walkBobAmount;
                targetBobSpeed = movementController.CharacterMovement.IsRunning ? data.runBobSpeed : data.walkBobSpeed;

                currentBobAmount = Lerp(currentBobAmount, targetBobAmount, data.transitionSpeed * Time.deltaTime);
                currentBobSpeed = Lerp(currentBobSpeed, targetBobSpeed, data.transitionSpeed * Time.deltaTime);

                wasMoving = true;
            }
            else if (wasMoving)
            {
                // Reset when stopping
                timer = 0f;
                currentBobAmount = Lerp(currentBobAmount, 0f, data.transitionSpeed * Time.deltaTime);
                if (currentBobAmount < 0.01f)
                {
                    currentBobAmount = 0f;
                    wasMoving = false;
                }
            }
        }

        public void HandleLandingEffect()
        {
            if (!movementController.CharacterMovement.JustLanded) return;
            timer = PI * 0.5f; // Start bob at bottom position
            currentBobAmount = data.landBobAmount;
        }

        public void ApplyHeadPosition()
        {
            switch (currentBobAmount)
            {
                case > 0f:
                    // Sine wave bob motion
                    CurrentYPos = defaultYPos + Sin(timer) * currentBobAmount;

                    Vector3 motion = default;
                    motion.x = headTransform.localPosition.x;
                    motion.y = CurrentYPos;
                    motion.z = headTransform.localPosition.z;

                    headTransform.localPosition = motion;
                    break;
                case < 0f:
                    // Smoothly return to default position
                    Vector3 defaultPos = default;
                    defaultPos.x = headTransform.localPosition.x;
                    defaultPos.y = defaultYPos;
                    defaultPos.z = headTransform.localPosition.z;

                    headTransform.localPosition = Vector3.Lerp(
                        headTransform.localPosition, defaultPos, data.landRecoverySpeed * Time.deltaTime);
                    break;
            }
        }

        public void HandleRotationTilt()
        {
            if (movementController.CharacterRotation.IsRotating)
            {
                var tiltDirection = movementController.CharacterRotation.TargetRotation.eulerAngles.y >
                    movementController.transform.eulerAngles.y ? 1f : -1f;
                var targetZRot = data.tiltAmount * tiltDirection;

                headTransform.localRotation = Quaternion.Lerp(
                    headTransform.localRotation,
                    Quaternion.Euler(headTransform.localEulerAngles.x, 0f, targetZRot),
                    data.tiltSpeed * Time.deltaTime
                );
            }
            else
                // Return to neutral
                headTransform.localRotation = Quaternion.Lerp(
                    headTransform.localRotation,
                    Quaternion.identity,
                    data.tiltSpeed * Time.deltaTime
                );
        }
    }
}