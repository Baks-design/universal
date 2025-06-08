using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputServicesManager : MonoBehaviour, IInputServices
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // Initialize input system first
            InputServiceProvider.InitializeMaps();
            PlayerMapInputProvider.InitializeActions();
            UIMapInputProvider.InitializeActions();

            // Then register the service
            ServiceLocator.Global.Register<IInputServices>(this);

            InputServiceProvider.EnablePlayerMap();
        }
    }
}
