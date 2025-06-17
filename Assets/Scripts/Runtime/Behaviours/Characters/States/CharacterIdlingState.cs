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
            controller.CharacterMovement.UpdateMovement();
            controller.CharacterRotation.UpdateRotation();

            if (!controller.CharacterMovement.IsMoving)
                controller.CharacterRotation.HandleRotationInput();
            if (!controller.CharacterRotation.IsRotating)
                controller.CharacterMovement.HandleGridMovement();
        }
    }
}