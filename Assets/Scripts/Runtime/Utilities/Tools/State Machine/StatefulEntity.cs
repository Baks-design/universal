using System;
using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    // <summary>
    /// Awake or Start can be used to declare all states and transitions.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override void Awake() {
    ///     base.Awake();
    /// 
    ///     var state = new State1(this);
    ///     var anotherState = new State2(this);
    ///
    ///     At(state, anotherState, () => true);
    ///     At(state, anotherState, myFunc);
    ///     At(state, anotherState, myPredicate);
    /// 
    ///     Any(anotherState, () => true);
    ///
    ///     stateMachine.SetState(state);
    /// </code> 
    /// </example>
    public abstract class StatefulEntity : MonoBehaviour
    {
        protected StateMachine stateMachine;

        protected virtual void Awake() => stateMachine = new StateMachine();

        protected virtual void FixedUpdate() => stateMachine.FixedUpdate();

        protected virtual void Update() => stateMachine.Update();

        protected virtual void LateUpdate() => stateMachine.LateUpdate();

        protected void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        protected void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

        protected void SetState(IState state) => stateMachine.SetState(state);
    }
}