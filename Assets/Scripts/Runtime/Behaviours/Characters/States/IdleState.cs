using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class IdleState : IState
    {
        readonly CharacterMovementController controller;

        public IdleState(CharacterMovementController controller) => this.controller = controller;

        public void Update()
        {
       
        }
    }
}