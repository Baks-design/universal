using UnityEngine;
using UnityUtils;

namespace Universal.Runtime.Utilities.Tools.ServiceLocator
{
    [DisallowMultipleComponent, RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        bool hasBeenBootstrapped;
        readonly ServiceLocator container;

        internal ServiceLocator Container
        {
            get
            {
                var res = container.OrNull();
                if (res == null)
                    TryGetComponent(out res);
                return res;
            }
        }

        void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped)
                return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}
