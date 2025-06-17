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
            if (controller.CameraController.IsBodyCameraEnabled)
                return;

            controller.CharacterMovement.UpdateMovement();
            controller.CharacterRotation.UpdateRotation();

            if (!controller.CharacterMovement.IsMoving)
                controller.CharacterRotation.HandleRotationInput();
            if (!controller.CharacterRotation.IsRotating)
                controller.CharacterMovement.HandleGridMovement();
        }
    }
}