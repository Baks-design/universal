using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveState : IState
    {
        readonly CharacterSwitchController controller;

        public MoveState(CharacterSwitchController controller) => this.controller = controller;

        public void Update()
        {
            controller.MovementController.CharacterMovement.HandleMovement(
                PlayerMapInputProvider.Move, Time.deltaTime
            );
        }
    }
}