using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterInvestigationState : IState
    {
        readonly IInputServices inputServices;

        public CharacterInvestigationState(IInputServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToInvestigateMap();
    }
}