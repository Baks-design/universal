using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly CharacterPlayerController controller;

        public CharacterMovementState(CharacterPlayerController controller) => this.controller = controller;

        public void OnEnter() => controller.InputServices.ChangeToMovementMap();

        public void FixedUpdate()
        {
            controller.MovementController.CharacterCollision.UpdateGroundStatus();
            controller.MovementController.CharacterMovement.MoveToTargetHandle();
            controller.MovementController.CharacterRotation.RotateToTarget();
        }

        public void LateUpdate()
        {
            controller.CameraController.CameraRotation.ReturnToInitialRotation();
            controller.MovementController.CharacterHeadBobbing.HandleRotationTilt();
            controller.MovementController.CharacterHeadBobbing.HandleMovementBob();
            controller.MovementController.CharacterHeadBobbing.ApplyHeadPosition();
            controller.MovementController.CharacterHeadBobbing.HandleLandingEffect();
        }
    }
}