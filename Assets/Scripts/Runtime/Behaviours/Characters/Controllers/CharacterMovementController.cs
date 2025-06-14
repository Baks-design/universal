using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Components.Camera;
using KBCore.Refs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour, IEnableComponent, IPlayableCharacter
    {
        [field: SerializeField, Self] public CharacterController Character { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public CharacterCameraController CameraController { get; private set; }
        [field: SerializeField] public Grid Grid { get; set; }
        [field: SerializeField, InLineEditor] public CharacterData Data { get; private set; }

        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public CharacterData CharacterData => Data;
        public Transform CharacterTransform => transform;
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

        void Awake() => InitializeComponents();

        void InitializeComponents()
        {
            CharacterMovement = new CharacterMovement(Character, Data, Grid, Camera.main);
            CharacterRotation = new CharacterRotation(this, Character, Data);
        }

        void Start() => CharacterMovement.SnapToGrid();

        void Update()
        {
            CharacterMovement.UpdateMovement();
            CharacterRotation.UpdateRotation();

            if (!CharacterMovement.IsMoving)
                CharacterRotation.HandleRotationInput();
            if (!CharacterRotation.IsRotating)
                CharacterMovement.HandleGridMovement();
        }
    }
}