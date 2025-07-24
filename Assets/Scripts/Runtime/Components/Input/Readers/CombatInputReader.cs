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
        [SerializeField, Self] InputServicesManager services;

        public Vector2 LookDirection => services.GameInputs.Combat.Look.ReadValue<Vector2>();

        public event Action OpenPauseScreen = delegate { };
        public event Action ToInvestigate = delegate { };
        public event Action ToMovement = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action Aim = delegate { };
        public event Action<bool> Attack = delegate { };
        public event Action<bool> AttackHold = delegate { };
        public event Action Reload = delegate { };

        void Awake() => ServiceLocator.Global.Register<ICombatInputReader>(this);

        public void OnLook(CallbackContext context) { }

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
            switch (context.phase)
            {
                case InputActionPhase.Started: Attack.Invoke(true); break;
                case InputActionPhase.Canceled: Attack.Invoke(false); break;
            }
            AttackHold.Invoke(context.interaction is HoldInteraction);
        }

        public void OnReload(CallbackContext context)
        {
            if (context.started) Reload.Invoke();
        }
    }
}