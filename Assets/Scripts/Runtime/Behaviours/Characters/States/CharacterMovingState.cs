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
            controller.CharacterMovement.HandleMovementInput();
            controller.CharacterMovement.UpdatePosition();
            controller.CharacterRotation.UpdateRotation();
        }
    }
}