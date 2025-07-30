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
    public class InvestigateInputReader : MonoBehaviour, IInvestigateActions, IInvestigateInputReader
    {
        [SerializeField, Self] InputReaderManager inputReader;

        public Vector2 LookDirection => inputReader.GameInputs.Investigate.Look.ReadValue<Vector2>();

        public event Action OpenPauseScreen = delegate { };
        public event Action ToMovement = delegate { };
        public event Action ToCombat = delegate { };
        public event Action AddCharacter = delegate { };
        public event Action NextCharacter = delegate { };
        public event Action PreviousCharacter = delegate { };
        public event Action Aim = delegate { };
        public event Action Interact = delegate { };
        public event Action RemoveCharacter = delegate { };

        void Awake() => ServiceLocator.Global.Register<IInvestigateInputReader>(this);
        
        public void OnLook(CallbackContext context) { }

        public void OnOpenPauseScreen(CallbackContext context)
        {
            if (context.performed) OpenPauseScreen.Invoke();
        }

        public void OnToMovement(CallbackContext context)
        {
            if (context.performed) ToMovement.Invoke();
        }

        public void OnToCombat(CallbackContext context)
        {
            if (context.performed) ToCombat.Invoke();
        }

        public void OnAddCharacter(CallbackContext context)
        {
            if (context.performed) AddCharacter.Invoke();
        }

        public void OnRemoveCharacter(CallbackContext context)
        {
            if (context.performed) RemoveCharacter.Invoke();
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

        public void OnInteract(CallbackContext context)
        {
            if (context.performed) Interact.Invoke();
        }
    }
}