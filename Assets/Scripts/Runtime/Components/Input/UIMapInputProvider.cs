using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class UIMapInputProvider
    {
        static InputAction unpause;

        public static InputAction Unpause => unpause;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeActions() => unpause = InputSystem.actions.FindAction("UI/Unpause");
    }
}