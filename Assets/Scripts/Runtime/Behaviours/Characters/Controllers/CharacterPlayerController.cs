using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Utilities.Tools.StateMachine;
using KBCore.Refs;
using Alchemy.Inspector;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Systems.InteractionObjects;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterPlayerController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] CharacterMovementController movementController;
        [SerializeField, Child] CharacterCameraController cameraController;
        [SerializeField, Child] PickupController pickupController;
        [SerializeField, Child] ThrowController throwController;
        [SerializeField, Child] CharacterDetectController detectController;
        [SerializeField, InlineEditor] CharacterData Data;
        ICharacterServices characterServices;
        IInvestigateInputReader investigateInput;
        IInputServices inputServices;
        IMovementInputReader movementInput;
        CharacterMovementState movementState;
        CharacterInvestigationState investigationState;
        bool isInInvestigatingState;
        bool isInMovementState;

        public Vector3Int CurrentGridPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public Transform CharacterTransform => tr;
        public CharacterData CharacterData => Data;
        public CharacterMovementController MovementController => movementController;
        public CharacterCameraController CameraController => cameraController;
        public CharacterDetectController CharacterDetectController => detectController;
        public ICharacterServices CharacterServices => characterServices;
        public IInvestigateInputReader InvestigateInput => investigateInput;
        public IInputServices InputServices => inputServices;

        #region Setup
        public void Initialize(CharacterData data) => Data = data;

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

        protected override void Awake()
        {
            base.Awake();
            ServiceLocator.Global.Get(out inputServices);
            ServiceLocator.Global.Get(out characterServices);
            ServiceLocator.Global.Get(out movementInput);
            ServiceLocator.Global.Get(out investigateInput);
        }

        void OnEnable()
        {
            movementInput.ToInvestigate += OnToInvestigator;
            movementInput.NextCharacter += OnNextCharacter;
            movementInput.PreviousCharacter += OnPreviousCharacter;
            movementInput.TurnRight += RightRotation;
            movementInput.TurnLeft += LeftRotation;
            movementInput.MoveForward += MoveForward;
            movementInput.MoveBackward += MoveBackward;
            movementInput.StrafeRight += StrafeRight;
            movementInput.StrafeLeft += StrafeLeft;
            movementInput.Crouch += Crouch;

            InvestigateInput.ToMovement += OnToMovement;
            InvestigateInput.AddCharacter += OnAddCharacter;
            InvestigateInput.RemoveCharacter += OnRemoveCharacter;
            InvestigateInput.NextCharacter += OnNextCharacter;
            InvestigateInput.PreviousCharacter += OnPreviousCharacter;
            InvestigateInput.Aim += OnAiming;
            InvestigateInput.Interact += OnPicking;
            InvestigateInput.Interact += OnThrowing;
        }

        void OnDisable()
        {
            movementInput.ToInvestigate -= OnToInvestigator;
            movementInput.NextCharacter -= OnNextCharacter;
            movementInput.PreviousCharacter -= OnPreviousCharacter;
            movementInput.TurnRight -= RightRotation;
            movementInput.TurnLeft -= LeftRotation;
            movementInput.MoveForward -= MoveForward;
            movementInput.MoveBackward -= MoveBackward;
            movementInput.StrafeRight -= StrafeRight;
            movementInput.StrafeLeft -= StrafeLeft;
            movementInput.Crouch -= Crouch;

            InvestigateInput.ToMovement -= OnToMovement;
            InvestigateInput.AddCharacter -= OnAddCharacter;
            InvestigateInput.RemoveCharacter -= OnRemoveCharacter;
            InvestigateInput.NextCharacter -= OnNextCharacter;
            InvestigateInput.PreviousCharacter -= OnPreviousCharacter;
            InvestigateInput.Aim -= OnAiming;
            InvestigateInput.Interact -= OnPicking;
            InvestigateInput.Interact -= OnThrowing;
        }

        void Start()
        {
            movementState = new CharacterMovementState(this);
            investigationState = new CharacterInvestigationState(this);

            At(movementState, investigationState, () => isInInvestigatingState);
            At(investigationState, movementState, () => isInMovementState);

            Set(movementState);
        }
        #endregion

        #region Shared
        void OnToInvestigator()
        {
            isInInvestigatingState = true;
            isInMovementState = false;
            CameraController.Cinemachine.Priority = 9;
        }

        void OnToMovement()
        {
            isInInvestigatingState = false;
            isInMovementState = true;
            CameraController.Cinemachine.Priority = 1;
        }

        void OnNextCharacter() => CharacterServices.NextCharacter();

        void OnPreviousCharacter() => CharacterServices.PreviousCharacter();
        #endregion

        #region Movement
        void RightRotation()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterRotation.RotationRight();
        }

        void LeftRotation()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterRotation.RotationLeft();
        }

        void MoveForward()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterMovement.MoveInDirection(tr.forward);
        }

        void MoveBackward()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterMovement.MoveInDirection(-tr.forward);
        }

        void StrafeRight()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterMovement.MoveInDirection(tr.right);
        }

        void StrafeLeft()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterMovement.MoveInDirection(-tr.right);
        }

        void Crouch() => movementController.CharacterCrouch.HandleCrouchInput();
        #endregion

        #region Inventigation
        void OnAiming()
        {
            if (stateMachine.CurrentState != investigationState) return;
            cameraController.CameraAiming.ChangeFOV(this);
        }

        void OnPicking()
        {
            if (stateMachine.CurrentState != investigationState) return;
            pickupController.OnInteractStarted();
        }

        void OnThrowing()
        {
            if (stateMachine.CurrentState != investigationState) return;
            throwController.OnThrowStarted();
        }

        void OnAddCharacter()
        {
            if (stateMachine.CurrentState != investigationState) return;
            detectController.OnAddCharacter();
        }

        void OnRemoveCharacter()
        {
            if (stateMachine.CurrentState != investigationState) return;
            CharacterServices.RemoveCharacterFromRoster(CharacterServices.GetCurrentCharacterIndex());
        }
        #endregion
    }
}