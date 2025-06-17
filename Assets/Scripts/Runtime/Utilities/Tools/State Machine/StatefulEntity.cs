using System;
using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public abstract class StatefulEntity : MonoBehaviour
    {
        protected StateMachine stateMachine;

        protected virtual void Awake() => stateMachine = new StateMachine();

        void FixedUpdate() => stateMachine.FixedUpdate();

        void Update() => stateMachine.Update();

        void LateUpdate() => stateMachine.LateUpdate();

        protected void At<T>(IState from, IState to, Func<T> condition) => stateMachine.AddTransition(from, to, condition);

        protected void Any<T>(IState to, Func<T> condition) => stateMachine.AddAnyTransition(to, condition);

        protected void Set(IState setState) => stateMachine.SetState(setState);
    }
}