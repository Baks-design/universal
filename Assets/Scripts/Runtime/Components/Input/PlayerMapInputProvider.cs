using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class PlayerMapInputProvider
    {
        static InputAction move;
        static InputAction look;
        static InputAction switchCharacter;
        static InputAction addCharacter;
        static InputAction pause;
        static InputAction setAttackMode;

        public static Vector2 MousePosition => Mouse.current.position.ReadValue();
        public static Vector2 Move => move?.ReadValue<Vector2>() ?? Vector2.zero;
        public static Vector2 Look => look?.ReadValue<Vector2>() ?? Vector2.zero;
        public static InputAction SwitchCharacter => switchCharacter;
        public static InputAction AddCharacter => addCharacter;
        public static InputAction Pause => pause;
        public static InputAction SetAttackMode => setAttackMode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeActions()
        {
            move = InputSystem.actions.FindAction("Player/Move");
            look = InputSystem.actions.FindAction("Player/Look");
            switchCharacter = InputSystem.actions.FindAction("Player/SwitchCharacter");
            addCharacter = InputSystem.actions.FindAction("Player/AddCharacter");
            pause = InputSystem.actions.FindAction("Player/Pause");
            setAttackMode = InputSystem.actions.FindAction("Player/SetAttackMode");
        }
    }
}