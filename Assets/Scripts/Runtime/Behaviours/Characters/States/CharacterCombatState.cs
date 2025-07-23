using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCombatState : IState
    {
        readonly IInputServices inputServices;

        public CharacterCombatState(IInputServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToCombatMap();
    }
}