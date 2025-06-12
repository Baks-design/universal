using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class PlayerMapInputProvider
    {
        public static InputAction Pause { get; private set; }
        public static InputAction AddCharacter { get; private set; }
        public static InputAction SwitchCharacter { get; private set; }
        public static InputAction SetAttackMode { get; private set; }
        public static InputAction Look { get; private set; }
        public static InputAction Turn { get; private set; }
        public static InputAction Move { get; private set; }
        public static InputAction Run { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeActions()
        {
            Pause = InputSystem.actions.FindAction("Player/Pause");
            AddCharacter = InputSystem.actions.FindAction("Player/AddCharacter");
            SwitchCharacter = InputSystem.actions.FindAction("Player/SwitchCharacter");
            SetAttackMode = InputSystem.actions.FindAction("Player/SetAttackMode");
            Look = InputSystem.actions.FindAction("Player/Look");
            Turn = InputSystem.actions.FindAction("Player/Turn");
            Move = InputSystem.actions.FindAction("Player/Move");
            Run = InputSystem.actions.FindAction("Player/Run");
        }
    }
}