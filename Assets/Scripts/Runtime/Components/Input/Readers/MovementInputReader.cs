using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class MovementInputReader : MonoBehaviour, IMovementActions, IMovementInputReader
    {
        [SerializeField, Self] InputReaderManager inputReader;

        public Vector2 MoveDirection => inputReader.GameInputs.Movement.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => inputReader.GameInputs.Movement.Look.ReadValue<Vector2>();

        public event Action OpenPauseScreen = delegate { };
        public event Action ToInvestigate = delegate { };
        public event Action ToCombat = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action Aim = delegate { };
        public event Action<bool> Run = delegate { };
        public event Action Crouch = delegate { };
        public event Action Jump = delegate { };

        void Awake() => ServiceLocator.Global.Register<IMovementInputReader>(this);

        public void OnMove(CallbackContext context) { }
        public void OnLook(CallbackContext context) { }

        public void OnOpenPauseScreen(CallbackContext context)
        {
            if (context.performed) OpenPauseScreen.Invoke();
        }

        public void OnToInvestigate(CallbackContext context)
        {
            if (context.performed) ToInvestigate.Invoke();
        }

        public void OnToCombat(CallbackContext context)
        {
            if (context.performed) ToCombat.Invoke();
        }

        public void OnNextCharacter(CallbackContext context)
        {
            if (context.performed) NextCharacter.Invoke();
        }

        public void OnPreviousCharacter(CallbackContext context)
        {
            if (context.performed) PreviousCharacter.Invoke();
        }

        public void OnAim(CallbackContext context)
        {
            if (context.interaction is not HoldInteraction) return;

            switch (context.phase)
            {
                case InputActionPhase.Started: Aim.Invoke(); break;
                case InputActionPhase.Canceled: Aim.Invoke(); break;
            }
        }

        public void OnRun(CallbackContext context)
        {
            Run.Invoke(context.interaction is HoldInteraction);
        }

        public void OnCrouch(CallbackContext context)
        {
            if (context.performed) Crouch.Invoke();
        }

        public void OnJump(CallbackContext context)
        {
            if (context.performed) Jump.Invoke();
        }
    }
}