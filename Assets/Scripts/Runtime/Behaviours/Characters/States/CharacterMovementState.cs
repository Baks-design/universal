using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly CharacterPlayerController controller;

        public CharacterMovementState(CharacterPlayerController controller)
        => this.controller = controller;

        public void OnEnter() => controller.InputServices.ChangeToMovementMap();
    }
}