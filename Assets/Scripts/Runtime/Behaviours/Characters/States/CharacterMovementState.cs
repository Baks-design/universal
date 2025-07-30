using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementState : IState
    {
        readonly IInputReaderServices inputServices;

        public CharacterMovementState(IInputReaderServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToMovementMap();
    }
}