using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public class InputServicesManager : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InputServiceProvider.SetCursorLocked(true);

            InputServiceProvider.InitializeMaps();
            PlayerMapInputProvider.InitializeActions();
            UIMapInputProvider.InitializeActions();
            InputServiceProvider.EnablePlayerMap();
        }
    }
}
