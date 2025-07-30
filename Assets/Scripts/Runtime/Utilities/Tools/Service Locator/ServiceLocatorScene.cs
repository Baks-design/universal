using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.ServicesLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorScene : Bootstrapper
    {
        protected override void Bootstrap() => Container.ConfigureForScene();
    }
}