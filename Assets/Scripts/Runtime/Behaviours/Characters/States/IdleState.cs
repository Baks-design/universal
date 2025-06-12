using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class IdleState : IState
    {
        readonly CharacterMovementController controller;

        public IdleState(CharacterMovementController controller) => this.controller = controller;

        public void Update()
        {
            // controller.CharacterMovement.HandleMovement( TODO: SETUP STATE
            //     PlayerMapInputProvider.Move, Time.deltaTime
            // );
        }
    }
}