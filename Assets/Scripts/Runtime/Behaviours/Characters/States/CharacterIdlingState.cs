using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterIdlingState : IState
    {
        public CharacterMovementController controller;

        public CharacterIdlingState(CharacterMovementController controller)
        => this.controller = controller;

        public void Update()
        {
            if (controller.CameraController.IsBodyCameraEnabled ||
                controller.CameraController.CameraRotation.IsRotatingBack)
                return;

            controller.CharacterMovement.UpdatePosition();
            controller.CharacterMovement.HandleMovementInput();

            controller.CharacterRotation.UpdateRotation();
            
            controller.CameraEffects.HandleCameraSway();
        }
    }
}