using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovingState : IState
    {
        public CharacterMovementController controller;

        public CharacterMovingState(CharacterMovementController controller)
        => this.controller = controller;

        public void Update()
        {
            if (controller.CameraController.IsBodyCameraEnabled ||
                controller.CameraController.CameraRotation.IsRotatingBack)
                return;

            controller.CharacterMovement.HandleMovementInput();
            controller.CharacterMovement.UpdatePosition();
            controller.CharacterRotation.UpdateRotation();
            controller.CameraEffects.HandleCameraSway();
        }
    }
}