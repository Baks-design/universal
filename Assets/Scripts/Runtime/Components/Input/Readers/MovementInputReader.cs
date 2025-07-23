using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class MovementInputReader : MonoBehaviour, IMovementActions, IMovementInputReader
    {
        public event Action OpenPauseScreen = delegate { };
        public event Action ToInvestigate = delegate { };
        public event Action ToCombat = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action<Vector2> Look = delegate { };
        public event Action Aim = delegate { };
        public event Action<Vector2> Move = delegate { };
        public event Action<bool> Run = delegate { };
        public event Action Crouch = delegate { };
        public event Action Jump = delegate { };

        void Awake() => ServiceLocator.Global.Register<IMovementInputReader>(this);

        public void OnOpenPauseScreen(CallbackContext context)
        {
            if (context.started) OpenPauseScreen.Invoke();
        }

        public void OnToInvestigate(CallbackContext context)
        {
            if (context.started) ToInvestigate.Invoke();
        }

        public void OnToCombat(CallbackContext context)
        {
            if (context.started) ToCombat.Invoke();
        }

        public void OnNextCharacter(CallbackContext context)
        {
            if (context.started) NextCharacter.Invoke();
        }

        public void OnPreviousCharacter(CallbackContext context)
        {
            if (context.started) PreviousCharacter.Invoke();
        }

        public void OnLook(CallbackContext context)
        => Look.Invoke(context.ReadValue<Vector2>());

        public void OnAim(CallbackContext context)
        {
            if (context.interaction is not HoldInteraction) return;

            switch (context.phase)
            {
                case InputActionPhase.Started: Aim.Invoke(); break;
                case InputActionPhase.Canceled: Aim.Invoke(); break;
            }
        }

        public void OnMove(CallbackContext context)
        => Move.Invoke(context.ReadValue<Vector2>());

        public void OnRun(CallbackContext context)
        {
            if (context.started) Run.Invoke(true);
            if (context.canceled) Run.Invoke(false);
        }

        public void OnCrouch(CallbackContext context)
        {
            if (context.started) Crouch.Invoke();
        }

        public void OnJump(CallbackContext context)
        {
            if (context.started) Jump.Invoke();
        }
    }
}