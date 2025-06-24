using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Utilities.Tools.StateMachine;
using KBCore.Refs;
using Alchemy.Inspector;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        [field: SerializeField, Self] public Transform Transform { get; private set; }
        [field: SerializeField, Child] public CharacterCameraController CameraController { get; private set; }
        [field: SerializeField, InlineEditor] public CharacterData Data { get; private set; }
        IPlayerInputReader inputReader;

        public CharacterData CharacterData => Data;
        public Transform CharacterTransform => Transform;
        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }
        public CameraEffects CameraEffects { get; private set; }
        public Grid Grid { get; private set; }
        public Vector3Int CurrentGridPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }

        protected override void Awake()
        {
            base.Awake();
            StateMachine();
        }

        void StateMachine()
        {
            var idlingState = new CharacterIdlingState(this);
            var movingState = new CharacterMovingState(this);

            At(idlingState, movingState, () => CharacterMovement.IsMoving);
            At(movingState, idlingState, () => !CharacterMovement.IsMoving);

            Set(idlingState);
        }

        void SetupMovement()
        {
            ServiceLocator.Global.Get(out inputReader);

            CharacterRotation = new CharacterRotation(this, Transform, Data);
            CharacterMovement = new CharacterMovement(this, Transform, Data, Grid, Camera.main, inputReader);
            CameraEffects = new CameraEffects(this, CameraController, inputReader);
        }

        void OnEnable()
        {
            SetupMovement();
            RegisterInputs();
        }

        void OnDisable() => UnregisterInputs();

        void RegisterInputs()
        {
            inputReader.TurnRight += RightRotationInput;
            inputReader.TurnLeft += LeftRotationInput;
        }

        void UnregisterInputs()
        {
            inputReader.TurnRight -= RightRotationInput;
            inputReader.TurnLeft -= LeftRotationInput;
        }

        void RightRotationInput() => CharacterRotation.HandleRotationRightInput();

        void LeftRotationInput() => CharacterRotation.HandleRotationLeftInput();

        void Start() => CharacterMovement.SnapToGrid();

        public void Initialize(CharacterData data, Grid grid)
        {
            Data = data;
            Grid = grid;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            Transform.SetLocalPositionAndRotation(LastPosition, LastRotation);
        }

        public void Deactivate()
        {
            LastPosition = Transform.localPosition;
            LastRotation = Transform.localRotation;
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            CharacterMovement.DrawMovementGizmos();
        }
#endif
    }
}