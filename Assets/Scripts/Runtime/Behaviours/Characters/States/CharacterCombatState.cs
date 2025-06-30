using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCombatState : IState
    {
        readonly PlayerController controller;

        public CharacterCombatState(PlayerController controller) => this.controller = controller;
    }
}