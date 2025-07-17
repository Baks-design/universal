using System;
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
        public event Action<Vector2> Move = delegate { };
        public event Action<bool> Run = delegate { };

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
        => Move.Invoke(context.ReadValue<Vector2>());

        public void OnRun(CallbackContext context)
        {
            if (context.started) Run.Invoke(true);
            if (context.canceled) Run.Invoke(false);
        }
    }
}