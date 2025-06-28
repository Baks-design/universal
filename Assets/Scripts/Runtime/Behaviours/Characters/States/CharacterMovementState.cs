using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly PlayerController controller;

        public CharacterMovementState(PlayerController controller) => this.controller = controller;

        public void LateUpdate() => controller.CameraController.CameraRotation.ReturnToInitialRotation(); //FIXME

        public void Update()
        {
            controller.MovementController.CharacterMovement.HandleMovementInput();
            controller.MovementController.CharacterMovement.UpdatePosition();
            controller.MovementController.CharacterRotation.UpdateRotation();
        }
    }
}