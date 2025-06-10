using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputServicesManager : MonoBehaviour, IInputServices
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IInputServices>(this);
            InputServiceProvider.SetCursorLocked(true);

            InputServiceProvider.InitializeMaps();
            PlayerMapInputProvider.InitializeActions();
            UIMapInputProvider.InitializeActions();
            InputServiceProvider.EnablePlayerMap();
        }
    }
}
