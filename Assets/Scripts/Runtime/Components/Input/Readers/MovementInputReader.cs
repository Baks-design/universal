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
        [SerializeField, Self] InputServicesManager inputServices;

        public Vector2 MoveDirection => inputServices.GameInputs.Movement.Move.ReadValue<Vector2>();

        public event Action OpenPauseScreen = delegate { };
        public event Action ToInvestigate = delegate { };
        public event Action ToCombat = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action TurnRight = delegate { };
        public event Action TurnLeft = delegate { };

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

        public void OnMove(CallbackContext context) { }

        public void OnTurnRight(CallbackContext context)
        {
            if (context.started) TurnRight.Invoke();
        }

        public void OnTurnLeft(CallbackContext context)
        {
            if (context.started) TurnLeft.Invoke();
        }
    }
}