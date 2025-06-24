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

        public event Action Pause = delegate { };
        public event Action AddCharacter = delegate { };
        public event Action SwitchCharacter = delegate { };
        public event Action Inspection = delegate { };
        public event Action<Vector2> Look = delegate { };
        public event Action Aim = delegate { };
        public event Action TurnRight = delegate { };
        public event Action TurnLeft = delegate { };
        public event Action<Vector2> Move = delegate { };
        public event Action Run = delegate { };
        public event Action Interact = delegate { };
        public event Action Throw = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };

        void Awake() => ServiceLocator.Global.Register<IPlayerInputReader>(this);

        public void OnMove(CallbackContext context) { }
        public void OnLook(CallbackContext context) { }
        
        public void OnPause(CallbackContext context)
        {
            if (context.started) Pause.Invoke();
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
            switch (context.ReadValue<float>())
            {
                case > 0f: TurnRight.Invoke(); break;
                case < 0f: TurnLeft.Invoke(); break;
            }
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