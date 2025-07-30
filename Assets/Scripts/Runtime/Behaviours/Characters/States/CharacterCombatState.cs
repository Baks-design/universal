using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCombatState : IState
    {
        readonly IInputReaderServices inputServices;

        public CharacterCombatState(IInputReaderServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToCombatMap();
    }
}