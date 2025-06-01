using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.Stats
{
    public class Bootstrapper : MonoBehaviour
    {
        void Awake() => ServiceLocator.Global.Register<IStatModifierFactory>(new StatModifierFactory());
    }
}