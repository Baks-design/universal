using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly IInputServices inputServices;

        public CharacterMovementState(IInputServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToMovementMap();
    }
}