using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class InputServiceProvider
    {
        static InputActionMap playerMap;
        static InputActionMap uiMap;

        public static InputActionMap PlayerMap => playerMap;
        public static InputActionMap UIMap => uiMap;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeMaps()
        {
            playerMap = InputSystem.actions.FindActionMap("Player");
            uiMap = InputSystem.actions.FindActionMap("UI");
        }

        public static void EnablePlayerMap()
        {
            uiMap.Disable();
            playerMap.Enable();
        }

        public static void EnableUIMap()
        {
            playerMap.Disable();
            uiMap.Enable();
        }

        public static void SetCursorLocked(bool isSet)
        => Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;
    }
}