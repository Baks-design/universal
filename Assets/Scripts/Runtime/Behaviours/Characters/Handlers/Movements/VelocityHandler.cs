using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class VelocityHandler
    {
        readonly CharacterMovementController movementController;
        readonly MovementSettings settings;
        readonly CharacterCollisionController collision;
        readonly CrouchHandler crouchHandler;
        readonly Transform camera;
        readonly IMovementInputReader movementInput;
        readonly float walkRunSpeedDifference;
        float currentSpeed;
        float smoothedSpeed;

        public Vector2 SmoothInputVector { get; private set; }
        public Vector3 FinalMoveDirection { get; private set; }
        public float FinalSmoothedSpeed { get; private set; }

        public VelocityHandler(
            CharacterMovementController movementController,
            MovementSettings settings,
            CharacterCollisionController collision,
            CrouchHandler crouchHandler,
            Transform camera,
            IMovementInputReader movementInput)
        {
            this.movementController = movementController;
            this.settings = settings;
            this.collision = collision;
            this.crouchHandler = crouchHandler;
            this.camera = camera;
            this.movementInput = movementInput;

            walkRunSpeedDifference = settings.runSpeed - settings.walkSpeed;
        }

        public void SmoothInput()
        => SmoothInputVector = Helpers.ExpDecay(
            SmoothInputVector, movementInput.MoveDirection.normalized, Time.deltaTime * settings.smoothInputSpeed);

        public void CalculateSpeed()
        {
            currentSpeed = movementInput.MoveDirection != Vector2.zero ?
                (crouchHandler.IsCrouching ? settings.crouchSpeed :
                    (movementController.IsRunning && CanRun() ? settings.runSpeed : settings.walkSpeed)) : 0f;

            if (movementInput.MoveDirection.y < -0.01f)
                currentSpeed *= settings.moveBackwardsSpeedPercent;
            else if (Abs(movementInput.MoveDirection.x) > 0.01f && Abs(movementInput.MoveDirection.y) < 0.01f)
                currentSpeed *= settings.moveSideSpeedPercent;
        }

        public void SmoothSpeed()
        {
            smoothedSpeed = Helpers.ExpDecay(smoothedSpeed, currentSpeed, Time.deltaTime * settings.smoothVelocitySpeed);

            if (movementController.IsRunning && CanRun())
            {
                var runTransitionProgress = Helpers.InverseDecay(settings.walkSpeed, settings.runSpeed, smoothedSpeed);
                FinalSmoothedSpeed = settings.runTransitionCurve.Evaluate(runTransitionProgress)
                    * walkRunSpeedDifference
                    + settings.walkSpeed;
            }
            else
                FinalSmoothedSpeed = smoothedSpeed;
        }

        public void CalculateMovementDirection()
        {
            var verticalDir = camera.transform.forward * SmoothInputVector.y;
            var horizontalDir = camera.transform.right * SmoothInputVector.x;
            var desiredDirection = (verticalDir + horizontalDir).normalized;

            FinalMoveDirection = collision.IsGrounded
                ? Vector3.ProjectOnPlane(desiredDirection, collision.GroundHit.normal).normalized
                : desiredDirection;
        }

        public bool CanRun()
        {
            if (FinalMoveDirection == Vector3.zero) return false;

            var forwardAlignment = Vector3.Dot(camera.transform.forward, FinalMoveDirection.normalized);
            return forwardAlignment >= settings.canRunThreshold && !crouchHandler.IsCrouching;
        }

        public void ApplySliding(ref Vector3 moveVector)
        {
            if (!collision.IsOnSteepSlope) return;

            moveVector = settings.slideSpeed * Time.deltaTime * collision.SlopeDirection;
        }
    }
}