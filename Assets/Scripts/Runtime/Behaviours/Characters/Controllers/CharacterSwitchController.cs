using System;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSwitchController : Character
    {
        [field: SerializeField, Self] public CharacterMovementController MovementController { get; set; }
        StateMachine stateMachine;

        public override void Activate()
        {
            base.Activate();
            SetupStateMachine();
        }

        public override void Deactivate() => base.Deactivate();

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            var idle = new IdleState(this);
            var moving = new MoveState(this);

            At(idle, moving, () => PlayerMapInputProvider.Move != Vector2.zero);
            At(moving, idle, () => PlayerMapInputProvider.Move == Vector2.zero);

            stateMachine.SetState(idle);
        }

        void At(IState from, IState to, Func<bool> condition)
        => stateMachine.AddTransition(from, to, condition);

        void Update()
        {
            stateMachine.Update();
            Debug.Log($"Current State: {stateMachine.CurrentState}");
        }
    }
}
