using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MovementCalculator
    {
        readonly MovementSettings settings;
        readonly CharacterCollisionController collision;
        readonly CrouchHandler crouchHandler;
        readonly CharacterController characterController;

        public Vector2 SmoothInputVector { get; private set; }
        public Vector3 FinalMoveDirection { get; private set; }
        public float CurrentSpeed { get; private set; }
        public float SmoothedSpeed { get; private set; }
        public float FinalSmoothedSpeed { get; private set; }
        public float WalkRunSpeedDifference { get; private set; }

        public MovementCalculator(
            MovementSettings settings,
            CharacterCollisionController collision,
            CrouchHandler crouchHandler,
            CharacterController characterController)
        {
            this.settings = settings;
            this.collision = collision;
            this.crouchHandler = crouchHandler;
            this.characterController = characterController;

            WalkRunSpeedDifference = settings.runSpeed - settings.walkSpeed;
        }

        public void UpdateMovement(InputMovement input)
        {
            SmoothInput(input);
            CalculateSpeed(input);
            SmoothSpeed(input);
            CalculateMovementDirection();
        }

        void SmoothInput(InputMovement input)
        => SmoothInputVector = Helpers.ExpDecay(
            SmoothInputVector, input.Move.normalized, Time.deltaTime * settings.smoothInputSpeed);

        void CalculateSpeed(InputMovement input)
        {
            // Base speed
            CurrentSpeed = input.HasInput ?
                (crouchHandler.IsCrouching ? settings.crouchSpeed :
                    (input.IsRunning && CanRun() ? settings.runSpeed : settings.walkSpeed)) : 0f;

            // Apply movement modifiers
            if (input.Move.y < -0.01f) 
                CurrentSpeed *= settings.moveBackwardsSpeedPercent;
            else if (Abs(input.Move.x) > 0.01f && Abs(input.Move.y) < 0.01f)
                CurrentSpeed *= settings.moveSideSpeedPercent;
        }

        void SmoothSpeed(InputMovement input)
        {
            SmoothedSpeed = Helpers.ExpDecay(SmoothedSpeed, CurrentSpeed, Time.deltaTime * settings.smoothVelocitySpeed);

            if (input.IsRunning && CanRun())
            {
                var runTransitionProgress = Helpers.InverseExpDecay(settings.walkSpeed, settings.runSpeed, SmoothedSpeed);
                FinalSmoothedSpeed = settings.runTransitionCurve.Evaluate(runTransitionProgress)
                    * WalkRunSpeedDifference
                    + settings.walkSpeed;
            }
            else
                FinalSmoothedSpeed = SmoothedSpeed;
        }

        void CalculateMovementDirection()
        {
            var verticalDir = characterController.transform.forward * SmoothInputVector.y;
            var horizontalDir = characterController.transform.right * SmoothInputVector.x;
            var desiredDirection = (verticalDir + horizontalDir).normalized;

            FinalMoveDirection = collision.IsGrounded
                ? Vector3.ProjectOnPlane(desiredDirection, collision.GroundHit.normal).normalized
                : desiredDirection;
        }

        public bool CanRun()
        {
            if (FinalMoveDirection == Vector3.zero) return false;
            
            var forwardAlignment = Vector3.Dot(characterController.transform.forward, FinalMoveDirection.normalized);
            return forwardAlignment >= settings.canRunThreshold && !crouchHandler.IsCrouching;
        }
    }
}