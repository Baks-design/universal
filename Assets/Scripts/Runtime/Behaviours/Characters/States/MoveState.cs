using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveState : IState
    {
        readonly CharacterMovementController controller;

        public MoveState(CharacterMovementController controller) => this.controller = controller;

        public void Update()
        {
            if (controller.CameraController.IsActiveCurrentState)
                return;

        }
    }
}