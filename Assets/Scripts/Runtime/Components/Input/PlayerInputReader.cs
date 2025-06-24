using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class PlayerInputReader : MonoBehaviour, IPlayerActions, IPlayerInputReader
    {
        [SerializeField] InputServicesManager inputServices;

        public Vector2 LookDirection => inputServices.gameInputs.Player.Look.ReadValue<Vector2>();
        public Vector2 MoveDirection => inputServices.gameInputs.Player.Move.ReadValue<Vector2>();
        public float Running => inputServices.gameInputs.Player.Run.ReadValue<float>();
        public float Turning => inputServices.gameInputs.Player.Turn.ReadValue<float>();

        public event Action Pause = delegate { };
        public event Action AddCharacter = delegate { };
        public event Action SwitchCharacter = delegate { };
        public event Action Inspection = delegate { };
        public event Action<Vector2, bool> Look = delegate { };
        public event Action Aim = delegate { };
        public event Action Turn = delegate { };
        public event Action<Vector2> Move = delegate { };
        public event Action Run = delegate { };
        public event Action Interact = delegate { };
        public event Action Throw = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };

        void Awake() => ServiceLocator.Global.Register<IPlayerInputReader>(this);

        public void OnPause(CallbackContext context)
        {
            if (context.started) Pause.Invoke();
        }
        public void OnMove(CallbackContext context)
        {
            if (context.performed || context.canceled)
                Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(CallbackContext context)
        {
            if (context.performed || context.canceled)
                Look.Invoke(context.ReadValue<Vector2>(), IsMouse(context));
        }

        static bool IsMouse(CallbackContext context)
        => context.control.device is Mouse;

        public void OnRun(CallbackContext context)
        {
            //TODO: Add 
            // switch (context.ReadValue<float>())
            // {
            //     case > 0f: NextCharacter.Invoke(); break;
            //     case < 0f: PreviousCharacter.Invoke(); break;
            // }
        }
        public void OnAddCharacter(CallbackContext context)
        {
            if (context.started) AddCharacter.Invoke();
        }
        public void OnSwitchCharacter(CallbackContext context)
        {
            switch (context.ReadValue<float>())
            {
                case > 0f: NextCharacter.Invoke(); break;
                case < 0f: PreviousCharacter.Invoke(); break;
            }
        }
        public void OnInspection(CallbackContext context)
        {
            if (context.started) Inspection.Invoke();
        }
        public void OnAim(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Aim.Invoke(); break;
                case InputActionPhase.Canceled: Aim.Invoke(); break;
            }
        }
        public void OnTurn(CallbackContext context)
        {
            //TODO: Add 
            // switch (context.ReadValue<float>())
            // {
            //     case > 0f: NextCharacter.Invoke(); break;
            //     case < 0f: PreviousCharacter.Invoke(); break;
            // }
        }
        public void OnInteract(CallbackContext context)
        {
            if (context.started) Interact.Invoke();
        }
        public void OnThrow(CallbackContext context)
        {
            if (context.started) Throw.Invoke();
        }
    }
}