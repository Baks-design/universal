using UnityEngine;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveHandler
    {
        readonly CharacterMovementController movementController;
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly MovementSettings settings;
        readonly CharacterCollisionController collision;
        readonly CrouchHandler crouchHandler;
        readonly IMovementInputReader movementInput;
        readonly VelocityHandler velocityHandler;
        readonly HeadBobHandler headBobHandler;
        readonly CharacterCameraController cameraController;
        Vector3 smoothedMoveDirection;
        Vector3 lastPosition;
        bool isInRunAnimation;
        public Vector3 FinalMoveVector;

        public bool IsMoving
        {
            get
            {
                var movement = controller.transform.localPosition - lastPosition;
                var isMove = Abs(movement.x) > Epsilon || Abs(movement.z) > Epsilon;
                lastPosition = controller.transform.localPosition;
                return isMove;
            }
        }
        
        public MoveHandler(
            CharacterMovementController movementController,
            CharacterController controller,
            Transform yawTransform,
            MovementSettings settings,
            CharacterCollisionController collision,
            VelocityHandler velocityHandler,
            HeadBobHandler headBobHandler,
            CharacterCameraController cameraController,
            CrouchHandler crouchHandler,
            IMovementInputReader movementInput)
        {
            this.movementController = movementController;
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.settings = settings;
            this.collision = collision;
            this.velocityHandler = velocityHandler;
            this.headBobHandler = headBobHandler;
            this.cameraController = cameraController;
            this.crouchHandler = crouchHandler;
            this.movementInput = movementInput;

            lastPosition = controller.transform.localPosition;
        }

        public void RotateTowardsCamera()
        => controller.transform.localRotation = Quaternion.Slerp(
            Quaternion.Euler(0f, controller.transform.localRotation.y, 0f),
            Quaternion.Euler(0f, yawTransform.localRotation.y, 0f),
            Time.deltaTime * settings.smoothRotateSpeed);

        public void SmoothMovementDirection()
        => smoothedMoveDirection = Helpers.ExpDecay(
            smoothedMoveDirection,
            velocityHandler.FinalMoveDirection,
            Time.deltaTime * settings.smoothFinalDirectionSpeed);

        public void CalculateFinalMovement()
        {
            var horizontalMovement = smoothedMoveDirection * velocityHandler.FinalSmoothedSpeed;
            FinalMoveVector = new Vector3(horizontalMovement.x, FinalMoveVector.y, horizontalMovement.z);
            if (controller.isGrounded) FinalMoveVector.y = horizontalMovement.y;
        }

        public void HandleHeadBob()
        {
            var shouldHeadBob = movementInput.MoveDirection != Vector2.zero && collision.IsGrounded &&
                !collision.HasObstacle && !crouchHandler.DuringCrouchAnimation;

            var baseHeight = crouchHandler.IsCrouching ? crouchHandler.CrouchCameraHeight : crouchHandler.InitialCameraHeight;

            if (shouldHeadBob)
            {
                var isRunning = movementController.IsRunning && velocityHandler.CanRun();
                headBobHandler.ScrollHeadBob(isRunning, crouchHandler.IsCrouching, movementInput.MoveDirection);

                var targetPosition = (controller.transform.up * baseHeight) + headBobHandler.FinalOffset;

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
                    var resetPosition = new Vector3(0f, baseHeight, 0f);
                    yawTransform.localPosition = Helpers.ExpDecay(
                        yawTransform.localPosition,
                        resetPosition,
                        Time.deltaTime * settings.smoothHeadBobSpeed
                    );
                }
            }
        }

        public void HandleRunFOV()
        {
            var shouldRun = movementInput.MoveDirection != Vector2.zero && collision.IsGrounded &&
                !collision.HasObstacle && movementController.IsRunning && velocityHandler.CanRun();

            if (shouldRun && !isInRunAnimation)
            {
                isInRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }
            else if ((!movementController.IsRunning ||
                movementInput.MoveDirection == Vector2.zero ||
                collision.HasObstacle) && isInRunAnimation)
            {
                isInRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }
    }
}