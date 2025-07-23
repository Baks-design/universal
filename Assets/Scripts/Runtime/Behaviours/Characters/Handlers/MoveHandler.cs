using UnityEngine;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveHandler
    {
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly MovementSettings settings;
        readonly CharacterCollisionController collision;
        readonly CrouchHandler crouchHandler;
        readonly MovementCalculator movementCalculator;
        readonly HeadBobHandler headBobHandler;
        readonly CharacterCameraController cameraController;
        Vector3 smoothedMoveDirection;
        bool isInRunAnimation;
        public Vector3 FinalMoveVector;

        public MoveHandler(
            CharacterController controller,
            Transform yawTransform,
            MovementSettings settings,
            CharacterCollisionController collision,
            MovementCalculator movementCalculator,
            HeadBobHandler headBobHandler,
            CharacterCameraController cameraController,
            CrouchHandler crouchHandler)
        {
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.settings = settings;
            this.collision = collision;
            this.movementCalculator = movementCalculator;
            this.headBobHandler = headBobHandler;
            this.cameraController = cameraController;
            this.crouchHandler = crouchHandler;
        }

        public void UpdateMovement(InputMovement input)
        {
            RotateTowardsCamera();
            SmoothMovementDirection();
            CalculateFinalMovement();
            HandleHeadBob(input);
            HandleRunFOV(input);
        }

        public void RotateTowardsCamera()
        {
            var currentYRotation = controller.transform.localEulerAngles.y;
            var targetYRotation = yawTransform.localEulerAngles.y;
            var smoothFactor = Time.deltaTime * settings.smoothRotateSpeed;

            var smoothedYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, smoothFactor);
            controller.transform.localEulerAngles = new Vector3(0f, smoothedYRotation, 0f);
        }

        public void SmoothMovementDirection()
        => smoothedMoveDirection = Helpers.ExpDecay(
            smoothedMoveDirection,
            movementCalculator.FinalMoveDirection,
            Time.deltaTime * settings.smoothFinalDirectionSpeed);

        public void CalculateFinalMovement()
        {
            var horizontalMovement = smoothedMoveDirection * movementCalculator.FinalSmoothedSpeed;
            FinalMoveVector = new Vector3(horizontalMovement.x, FinalMoveVector.y, horizontalMovement.z);

            if (controller.isGrounded)
                FinalMoveVector.y = horizontalMovement.y;
        }

        public void HandleHeadBob(InputMovement input)
        {
            var shouldHeadBob = input.HasInput && collision.IsGrounded &&
                !collision.HasObstacle && !crouchHandler.DuringCrouchAnimation;

            if (shouldHeadBob)
            {
                var isRunning = input.IsRunning && movementCalculator.CanRun();
                headBobHandler.ScrollHeadBob(isRunning, crouchHandler.IsCrouching, input.Move);

                var targetPosition = (controller.transform.up * headBobHandler.CurrentStateHeight) +
                    headBobHandler.FinalOffset;

                yawTransform.localPosition = Helpers.ExpDecay(
                    yawTransform.localPosition,
                    targetPosition,
                    Time.deltaTime * settings.smoothHeadBobSpeed
                );
            }
            else
            {
                if (!headBobHandler.Resetted)
                    headBobHandler.ResetHeadBob();

                if (!crouchHandler.DuringCrouchAnimation)
                {
                    var resetPosition = new Vector3(0f, headBobHandler.CurrentStateHeight, 0f);
                    yawTransform.localPosition = Helpers.ExpDecay(
                        yawTransform.localPosition,
                        resetPosition,
                        Time.deltaTime * settings.smoothHeadBobSpeed
                    );
                }
            }
        }

        public void HandleRunFOV(InputMovement input)
        {
            var shouldRun = input.HasInput && collision.IsGrounded &&
                !collision.HasObstacle && input.IsRunning && movementCalculator.CanRun();

            if (shouldRun && !isInRunAnimation)
            {
                isInRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }
            else if ((!input.IsRunning || !input.HasInput || collision.HasObstacle) && isInRunAnimation)
            {
                isInRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }
    }
}