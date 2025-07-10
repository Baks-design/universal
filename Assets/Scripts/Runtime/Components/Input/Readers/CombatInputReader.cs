using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class CombatInputReader : MonoBehaviour, ICombatActions, ICombatInputReader
    {
        [SerializeField, Self] InputServicesManager inputServices;

        public event Action OpenPauseScreen = delegate { };
        public event Action ToInvestigate = delegate { };
        public event Action ToMovement = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action<Vector2> Look = delegate { };
        public event Action Aim = delegate { };
        public event Action Attack = delegate { };
        public event Action Target = delegate { };
        public event Action<float> Selection = delegate { };

        void Awake() => ServiceLocator.Global.Register<ICombatInputReader>(this);

        public void OnOpenPauseScreen(CallbackContext context)
        {
            if (context.started) OpenPauseScreen.Invoke();
        }

        public void OnToInvestigate(CallbackContext context)
        {
            if (context.started) ToInvestigate.Invoke();
        }

        public void OnToMovement(CallbackContext context)
        {
            if (context.started) ToMovement.Invoke();
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

        public void OnAttack(CallbackContext context)
        {
            if (context.started) Attack.Invoke();
        }

        public void OnTarget(CallbackContext context)
        {
            if (context.started) Target.Invoke();
        }

        public void OnSelection(CallbackContext context)
        {
            var value = context.ReadValue<float>();
            if (value != 0f) Selection.Invoke(value);
        }
    }
}