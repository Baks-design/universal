using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCombatState : IState
    {
        readonly CharacterPlayerController controller;

        public CharacterCombatState(CharacterPlayerController controller)
        => this.controller = controller;

        public void OnEnter() => controller.InputServices.ChangeToCombatMap();
    }
}