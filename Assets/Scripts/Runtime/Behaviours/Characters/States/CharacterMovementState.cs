using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly PlayerController controller;

        public CharacterMovementState(PlayerController controller) => this.controller = controller;

        public void OnEnter() => controller.inputServices.ChangeToMovementMap();

        public void Update()
        {
            controller.MovementController.CharacterMovement.HandleMovementInput();
            controller.MovementController.CharacterMovement.UpdatePosition();
            controller.MovementController.CharacterRotation.UpdateRotation();
        }

        public void LateUpdate() => controller.CameraController.CameraRotation.ReturnToInitialRotation(); 
    }
}