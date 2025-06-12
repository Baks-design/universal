using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public class Not : IPredicate
    {
        [SerializeField, TextArea] IPredicate rule;
        public bool Evaluate() => !rule.Evaluate();
    }
}