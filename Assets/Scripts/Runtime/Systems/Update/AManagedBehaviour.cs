using UnityEngine;
using Universal.Runtime.Systems.Update.Extensions;
using Universal.Runtime.Systems.Update.Interface;

namespace Universal.Runtime.Systems.Update
{
    /// <summary>
    /// <see cref="MonoBehaviour"/> abstract class with automatic registration in <see cref="UpdateManager"/>.
    /// </summary>
    /// <remarks>
    /// Instances will register themselves for updates in the <c>OnEnable</c> message and unregister in the 
    /// <c>OnDisable</c> message.
    /// <br/>
    /// Implement <see cref="IUpdatable"/>, <see cref="ILateUpdatable"/> and/or <see cref="IFixedUpdatable"/> 
    /// to be updated every frame using <see cref="UpdateManager"/>.
    /// </remarks>
    /// <seealso cref="UpdateManager"/>
    public abstract class AManagedBehaviour : MonoBehaviour, IManagedObject
    {
        protected virtual void OnEnable() => this.RegisterInManager();

        protected virtual void OnDisable() => this.UnregisterInManager();
    }
}
