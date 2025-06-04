using UnityEngine;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public abstract class AManagedBehaviour : MonoBehaviour, IManagedObject
    {
        protected virtual void OnEnable()
        {
            if (!ApplicationUtils.IsQuitting)
                this.RegisterInManager();
        }

        protected virtual void OnDisable()
        {
            if (!ApplicationUtils.IsQuitting && this != null)
                this.UnregisterInManager();
        }

        protected virtual void OnDestroy()
        {
            if (!ApplicationUtils.IsQuitting && this != null)
                this.UnregisterInManager();
        }
    }
}
