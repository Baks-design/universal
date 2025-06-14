using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.StateMachine;
using UnityEngine.InputSystem;
using Universal.Runtime.Components.Camera;
using KBCore.Refs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        [field: SerializeField, Self] public CharacterController Character { get; }
        [field: SerializeField] public Transform Transform { get; }
        [field: SerializeField] public CharacterCameraController Camera { get; }
        [field: SerializeField, InLineEditor] public CharacterData Data { get; private set; }

        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public CharacterCameraController CameraController => Camera;
        public CharacterData CharacterData => Data;
        public Transform CharacterTransform => Transform;
        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }
        public bool IsRunHold { get; private set; }

        public void Initialize(CharacterData data) => Data = data;

        public void Activate()
        {
            gameObject.SetActive(true);
            Transform.SetPositionAndRotation(LastPosition, LastRotation);
        }

        public void Deactivate()
        {
            LastPosition = Transform.position;
            LastRotation = Transform.rotation;
            gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeComponents();
            SetupStateMachine();
        }

        void InitializeComponents()
        {
            CharacterMovement = new CharacterMovement(this, Character, Data);
            CharacterRotation = new CharacterRotation(this, Character, Data);
        }

        void SetupStateMachine()
        {
            var idle = new IdleState(this);
            var moving = new MoveState(this);

            At(idle, moving, () => CharacterMovement.IsMoving);
            At(moving, idle, () => !CharacterMovement.IsMoving);

            SetState(idle);
        }

        void OnEnable() => SubscribeInputActions();

        void SubscribeInputActions()
        {
            PlayerMapInputProvider.Move.started += HandleMovementInput;
            PlayerMapInputProvider.Run.performed += HandleRunInput;
        }

        void OnDisable() => UnsubscribeInputActions();

        void OnDestroy() => UnsubscribeInputActions();

        void UnsubscribeInputActions()
        {
            PlayerMapInputProvider.Move.started -= HandleMovementInput;
            PlayerMapInputProvider.Run.performed -= HandleRunInput;
        }

        void HandleMovementInput(InputAction.CallbackContext context)
        {
            // Handle forward/backward movement
            switch (context.ReadValue<Vector2>().y)
            {
                case > 0f:
                    break;
                case < 0f:
                    break;
            }
            // Handle strafing
            switch (context.ReadValue<Vector2>().x)
            {
                case > 0f:
                    break;
                case < 0f:
        
                    break;
            }
        }

        void HandleRunInput(InputAction.CallbackContext context)
            => IsRunHold = context.phase is InputActionPhase.Performed;
    }
}