using UnityEngine;
using UnityUtils;

namespace Universal.Runtime.Utilities.Tools.ServicesLocator
{
    [DisallowMultipleComponent, RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        bool hasBeenBootstrapped;
        ServiceLocator container;
        internal ServiceLocator Container
        {
            get
            {
                var result = container.OrNull();
                if (result == null)
                {
                    container = GetComponent<ServiceLocator>();
                    result = container;
                }
                return result;
            }
        }

        void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}
