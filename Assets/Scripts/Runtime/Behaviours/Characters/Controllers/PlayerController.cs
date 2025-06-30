using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Utilities.Tools.StateMachine;
using KBCore.Refs;
using Alchemy.Inspector;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Systems.InteractionObjects;
using Logger = Universal.Runtime.Utilities.Helpers.Logger;

namespace Universal.Runtime.Behaviours.Characters
{
    public class PlayerController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] CharacterMovementController movementController;
        [SerializeField, Child] CharacterCameraController cameraController;
        [SerializeField, Child] PickupController pickupController;
        [SerializeField, Child] ThrowController throwController;
        [SerializeField, InlineEditor] CharacterData Data;
        Grid grid;
        IPlayerInputReader input;
        CharacterMovementState movementState;
        CharacterInvestigationState investigationState;
        bool IsInspectionStateEnabled = false;

        public Vector3Int CurrentGridPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public CharacterData CharacterData => Data;
        public Transform CharacterTransform => tr;
        public Grid Grid => grid;
        public CharacterMovementController MovementController => movementController;
        public CharacterCameraController CameraController => cameraController;

        public void Initialize(CharacterData data, Grid grid)
        {
            Data = data;
            this.grid = grid;
        }

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

        void Start()
        {
            StateMachine();
            SnapToGrid();
        }

        void StateMachine()
        {
            movementState = new CharacterMovementState(this);
            investigationState = new CharacterInvestigationState(this);

            At(movementState, investigationState, () => IsInspectionStateEnabled);
            At(investigationState, movementState, () => !IsInspectionStateEnabled);

            Set(movementState);
        }

        void OnEnable()
        {
            ServiceLocator.Global.Get(out input);
            input.Inspection += OnInspection;
            input.TurnRight += RightRotationInput;
            input.TurnLeft += LeftRotationInput;
            input.Aim += OnAiming;
            input.Interact += OnPicking;
            input.Throw += OnThrowing;
        }

        void OnDisable()
        {
            input.Inspection -= OnInspection;
            input.TurnRight -= RightRotationInput;
            input.TurnLeft -= LeftRotationInput;
            input.Aim -= OnAiming;
            input.Interact -= OnPicking;
            input.Throw -= OnThrowing;
        }

        void OnInspection()
        {
            IsInspectionStateEnabled = !IsInspectionStateEnabled;
            CameraController.Cinemachine.Priority = IsInspectionStateEnabled ? 9 : 1;
        }

        void RightRotationInput()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterRotation.HandleRotationRightInput();
        }

        void LeftRotationInput()
        {
            if (stateMachine.CurrentState != movementState) return;
            movementController.CharacterRotation.HandleRotationLeftInput();
        }

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

        void SnapToGrid()
        {
            CurrentGridPosition = grid.WorldToCell(tr.position);
            tr.position = grid.GetCellCenterWorld(CurrentGridPosition);
            MovementController.CharacterMovement.FacingDirection = tr.forward;
        }

        // protected override void Update()
        // {
        //     base.Update();
        //     Logger.Info($"Current Player State: {stateMachine.CurrentState}");
        // }
    }
}