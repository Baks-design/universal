using System;
using KBCore.Refs;
using UnityEngine;
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
        public event Action TurnRight = delegate { };
        public event Action TurnLeft = delegate { };
        public event Action MoveForward = delegate { };
        public event Action MoveBackward = delegate { };
        public event Action StrafeRight = delegate { };
        public event Action StrafeLeft = delegate { };
        public event Action Crouch = delegate { };

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

        public void OnTurnRight(CallbackContext context)
        {
            if (context.started) TurnRight.Invoke();
        }

        public void OnTurnLeft(CallbackContext context)
        {
            if (context.started) TurnLeft.Invoke();
        }

        public void OnMove(CallbackContext context)
        {
            if (context.ReadValue<Vector2>().y > 0f) MoveForward.Invoke();
            if (context.ReadValue<Vector2>().y < 0f) MoveBackward.Invoke();
            if (context.ReadValue<Vector2>().x > 0f) StrafeRight.Invoke();
            if (context.ReadValue<Vector2>().x < 0f) StrafeLeft.Invoke();
        }

        public void OnCrouch(CallbackContext context)
        {
            if (context.started) Crouch.Invoke();
        }
    }
}