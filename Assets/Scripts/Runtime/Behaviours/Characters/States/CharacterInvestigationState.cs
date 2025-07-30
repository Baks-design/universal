using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterInvestigationState : IState
    {
        readonly IInputReaderServices inputServices;

        public CharacterInvestigationState(IInputReaderServices inputServices)
        => this.inputServices = inputServices;

        public void OnEnter() => inputServices.ChangeToInvestigateMap();
    }
}