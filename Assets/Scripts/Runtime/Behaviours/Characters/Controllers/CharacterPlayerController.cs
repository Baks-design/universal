using UnityEngine;
using KBCore.Refs;
using Alchemy.Inspector;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.StateMachine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Unity.Cinemachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterPlayerController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        public IInputServices InputServices;
        [SerializeField, Self] Transform tr;
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, InlineEditor] CharacterData data;
        ICharacterServices characterServices;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        bool isInInvestigatingState;
        bool isInMovementState;
        bool isInCombatState;

        public Vector3Int CurrentGridPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public Transform CharacterTransform => tr;
        public CharacterData CharacterData => data;
        public CharacterMovementState MovementState { get; private set; }
        public CharacterInvestigationState InvestigationState { get; private set; }
        public CharacterCombatState CombatState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Global.Get(out characterServices);
            ServiceLocator.Global.Get(out InputServices);
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
            cinemachine.Priority = 1;
        }

        void OnToInvestigator()
        {
            isInMovementState = false;
            isInInvestigatingState = true;
            isInCombatState = false;
            cinemachine.Priority = 9;
        }

        void OnToCombat()
        {
            isInMovementState = false;
            isInInvestigatingState = false;
            isInCombatState = true;
            cinemachine.Priority = 9;
        }

        void OnNextCharacter() => characterServices.NextCharacter();

        void OnPreviousCharacter() => characterServices.PreviousCharacter();

        void Start()
        {
            MovementState = new CharacterMovementState(this);
            InvestigationState = new CharacterInvestigationState(this);
            CombatState = new CharacterCombatState(this);

            At(MovementState, InvestigationState, () => isInInvestigatingState);
            At(MovementState, CombatState, () => isInCombatState);

            At(InvestigationState, MovementState, () => isInMovementState);
            At(InvestigationState, CombatState, () => isInCombatState);

            At(CombatState, MovementState, () => isInMovementState);
            At(CombatState, InvestigationState, () => isInInvestigatingState);

            Set(MovementState);
        }

        public void Initialize(CharacterData data) => this.data = data;

        public void Activate()
        {
            gameObject.SetActive(true);
            tr.SetLocalPositionAndRotation(LastPosition, LastRotation);
        }

        public void Deactivate()
        {
            LastPosition = tr.localPosition;
            LastRotation = tr.localRotation;
            gameObject.SetActive(false);
        }

        void OnGUI()
        {
            GUIScaler.BeginScaledGUI();

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)(32 * GUIScaler.GetCurrentScale()),
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