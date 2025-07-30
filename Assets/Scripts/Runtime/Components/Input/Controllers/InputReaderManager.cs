using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputReaderManager : MonoBehaviour, IInputReaderServices
    {
        [SerializeField, Self] MovementInputReader movementInput;
        [SerializeField, Self] InvestigateInputReader investigateInput;
        [SerializeField, Self] CombatInputReader combatInput;
        [SerializeField, Self] UIInputReader uiInput;

        public GameInputs GameInputs { get; set; }

        void Awake()
        {
            ServiceLocator.Global.Register<IInputReaderServices>(this);
            SetupActions();
            SetCursorLocked(true);
        }

        void SetupActions()
        {
            GameInputs = new GameInputs();
            GameInputs.Movement.SetCallbacks(movementInput);
            GameInputs.Investigate.SetCallbacks(investigateInput);
            GameInputs.Combat.SetCallbacks(combatInput);
            GameInputs.UI.SetCallbacks(uiInput);
        }

        public void ChangeToMovementMap()
        {
            GameInputs.Movement.Enable();
            GameInputs.Investigate.Disable();
            GameInputs.Combat.Disable();
            GameInputs.UI.Disable();
        }

        public void ChangeToInvestigateMap()
        {
            GameInputs.Movement.Disable();
            GameInputs.Investigate.Enable();
            GameInputs.Combat.Disable();
            GameInputs.UI.Disable();
        }

        public void ChangeToCombatMap()
        {
            GameInputs.Movement.Disable();
            GameInputs.Investigate.Disable();
            GameInputs.Combat.Enable();
            GameInputs.UI.Disable();
        }

        public void ChangeToUIMap()
        {
            GameInputs.Movement.Disable();
            GameInputs.Investigate.Disable();
            GameInputs.Combat.Disable();
            GameInputs.UI.Enable();
        }

        public void DisableGameInput() => GameInputs.Disable();

        public void SetCursorLocked(bool isSet)
        => Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;
    }
}