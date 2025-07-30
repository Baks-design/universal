using UnityEngine;
using Alchemy.Inspector;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.StateMachine;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterPlayerController : StatefulEntity
    {
        [SerializeField, InlineEditor] CharacterSettings settings;
        ICharacterServices characterServices;
        IInputReaderServices inputServices;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        bool isInInvestigatingState;
        bool isInMovementState;
        bool isInCombatState;

        public CharacterSettings Settings => settings;
        public CharacterMovementState MovementState { get; private set; }
        public CharacterInvestigationState InvestigationState { get; private set; }
        public CharacterCombatState CombatState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            ServiceLocator.Global.Get(out characterServices);
            ServiceLocator.Global.Get(out inputServices);
            ServiceLocator.Global.Get(out movementInput);
            ServiceLocator.Global.Get(out investigateInput);
            ServiceLocator.Global.Get(out combatInput);
        }

        void OnEnable()
        {
            movementInput.ToCombat += OnToCombat;
            movementInput.ToInvestigate += OnToInvestigator;
            movementInput.NextCharacter += OnNextCharacter;
            movementInput.PreviousCharacter += OnPreviousCharacter;

            investigateInput.ToCombat += OnToCombat;
            investigateInput.ToMovement += OnToMovement;
            investigateInput.NextCharacter += OnNextCharacter;
            investigateInput.PreviousCharacter += OnPreviousCharacter;

            combatInput.ToInvestigate += OnToInvestigator;
            combatInput.ToMovement += OnToMovement;
            combatInput.NextCharacter += OnNextCharacter;
            combatInput.PreviousCharacter += OnPreviousCharacter;
        }

        void OnDisable()
        {
            movementInput.ToCombat -= OnToCombat;
            movementInput.ToInvestigate -= OnToInvestigator;
            movementInput.NextCharacter -= OnNextCharacter;
            movementInput.PreviousCharacter -= OnPreviousCharacter;

            investigateInput.ToCombat -= OnToCombat;
            investigateInput.ToMovement -= OnToMovement;
            investigateInput.NextCharacter -= OnNextCharacter;
            investigateInput.PreviousCharacter -= OnPreviousCharacter;

            combatInput.ToInvestigate -= OnToInvestigator;
            combatInput.ToMovement -= OnToMovement;
            combatInput.NextCharacter -= OnNextCharacter;
            combatInput.PreviousCharacter -= OnPreviousCharacter;
        }

        void OnToMovement()
        {
            isInMovementState = true;
            isInInvestigatingState = false;
            isInCombatState = false;
        }

        void OnToInvestigator()
        {
            isInMovementState = false;
            isInInvestigatingState = true;
            isInCombatState = false;
        }

        void OnToCombat()
        {
            isInMovementState = false;
            isInInvestigatingState = false;
            isInCombatState = true;
        }

        void OnNextCharacter() => characterServices.NextCharacter();

        void OnPreviousCharacter() => characterServices.PreviousCharacter();

        void Start()
        {
            MovementState = new CharacterMovementState(inputServices);
            InvestigationState = new CharacterInvestigationState(inputServices);
            CombatState = new CharacterCombatState(inputServices);

            At(MovementState, InvestigationState, () => isInInvestigatingState);
            At(MovementState, CombatState, () => isInCombatState);

            At(InvestigationState, MovementState, () => isInMovementState);
            At(InvestigationState, CombatState, () => isInCombatState);

            At(CombatState, MovementState, () => isInMovementState);
            At(CombatState, InvestigationState, () => isInInvestigatingState);

            Set(MovementState);
        }

        void OnGUI()
        {
            GUIScaler.BeginScaledGUI();

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)(32f * GUIScaler.GetCurrentScale()),
                normal = { textColor = Color.white }
            };

            GUIScaler.DrawProportionalLabel(
                new Vector2(0f, 50f),
                $"Current Player State: {stateMachine.CurrentState}",
                style);

            GUI.matrix = Matrix4x4.identity;
        }
    }
}