using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class IdleState : IState
    {
        readonly CharacterSwitchController controller;

        public IdleState(CharacterSwitchController controller) => this.controller = controller;

        public void Update()
        {
            controller.MovementController.CharacterMovement.HandleMovement(
                PlayerMapInputProvider.Move, Time.deltaTime
            );
        }
    }
}