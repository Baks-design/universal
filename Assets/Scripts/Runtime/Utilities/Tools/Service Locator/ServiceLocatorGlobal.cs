using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.ServicesLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobal : Bootstrapper
    {
        [SerializeField] bool dontDestroyOnLoad = true;

        protected override void Bootstrap() => Container.ConfigureAsGlobal(dontDestroyOnLoad);
    }
}