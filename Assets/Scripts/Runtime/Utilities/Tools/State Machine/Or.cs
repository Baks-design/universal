using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public class Or : IPredicate
    {
        [SerializeField] List<IPredicate> rules = new List<IPredicate>();
        public bool Evaluate() => rules.Any(r => r.Evaluate());
    }
}