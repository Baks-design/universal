using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Utilities.Tools.StateMachine;
using KBCore.Refs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : StatefulEntity, IEnableComponent, IPlayableCharacter
    {
        [field: SerializeField, Self] public Transform Transform { get; private set; }
        [field: SerializeField, Child] public CharacterCameraController CameraController { get; private set; }
        [field: SerializeField, InLineEditor] public CharacterData Data { get; private set; }

        public CharacterData CharacterData => Data;
        public Transform CharacterTransform => Transform;
        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }
        public Grid Grid { get; private set; }
        public Vector3Int CurrentGridPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }

        void Start()
        {
            Init();
            StateMachine();
            SnapToGrid();
        }

        void Init()
        {
            CharacterRotation = new CharacterRotation(this, Transform, Data);
            CharacterMovement = new CharacterMovement(this, Transform, Data, Grid, Camera.main);
        }

        void StateMachine()
        {
            var idlingState = new CharacterIdlingState(this);
            var movingState = new CharacterMovingState(this);

            At(idlingState, movingState, () => CharacterMovement.IsMoving);
            At(movingState, idlingState, () => !CharacterMovement.IsMoving);

            Set(idlingState);
        }

        void SnapToGrid()
        {
            CurrentGridPosition = Grid.WorldToCell(Transform.position);
            Transform.position = Grid.GetCellCenterWorld(CurrentGridPosition);
            CharacterMovement.FacingDirection = Transform.forward;
        }

        public void Initialize(CharacterData data, Grid grid)
        {
            Data = data;
            Grid = grid;
        }

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
    }
}