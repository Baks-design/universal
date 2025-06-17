using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public class And : IPredicate
    {
        [SerializeField] List<IPredicate> rules = new();

        public bool Evaluate() => rules.All(r => r.Evaluate());
    }
}