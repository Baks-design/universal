using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour, IEnableComponent, IPlayableCharacter
    {
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Child] CharacterCameraController cameraController;
        [SerializeField, Self] CharacterCollisionController collisionController;
        [SerializeField] Transform yawTransform;
        [SerializeField] HeadBobSettings headBobSettings;
        [SerializeField] MovementSettings movementSettings;
        CharacterSettings settings;
        IMovementInputReader input;
        CrouchHandler crouchHandler;
        LandingHandler landingHandler;
        MoveHandler moveHandler;
        VelocityHandler velocityHandler;
        HeadBobHandler headBobHandler;
        GravityHandler gravityHandler;

        public CharacterSettings Settings => settings;
        public Transform CharacterTransform => controller.transform;
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public bool IsMoving => controller.velocity.sqrMagnitude > 0.1f;
        public bool IsRunning { get; private set; }
        public Vector3 Direction => velocityHandler.FinalMoveDirection;

        void Awake()
        {
            ServiceLocator.Global.Get(out input);
            InitializeHandlers();
        }

        void InitializeHandlers()
        {
            landingHandler = new LandingHandler(this, yawTransform, movementSettings, collisionController);
            headBobHandler = new HeadBobHandler(
                headBobSettings, movementSettings.moveBackwardsSpeedPercent, movementSettings.moveSideSpeedPercent);
            crouchHandler = new CrouchHandler(this, controller, yawTransform, movementSettings, collisionController);
            gravityHandler = new GravityHandler(movementSettings, collisionController);
            velocityHandler = new VelocityHandler(
                this, movementSettings, collisionController, crouchHandler, yawTransform, input);
            moveHandler = new MoveHandler(this, controller, yawTransform, movementSettings, collisionController,
                velocityHandler, headBobHandler, cameraController, crouchHandler, input);
        }

        void OnEnable()
        {
            input.Run += OnGetRun;
            input.Crouch += OnGetCrouch;
        }

        void OnDisable()
        {
            input.Run -= OnGetRun;
            input.Crouch -= OnGetCrouch;
        }

        void OnGetRun(bool value) => IsRunning = value;

        void OnGetCrouch() => crouchHandler.OnCrouchInput();

        void Update()
        {
            moveHandler.RotateTowardsCamera();

            crouchHandler.UpdateCrouch();

            velocityHandler.SmoothInput();
            velocityHandler.CalculateSpeed();
            velocityHandler.SmoothSpeed();
            velocityHandler.CalculateMovementDirection();

            moveHandler.SmoothMovementDirection();
            moveHandler.CalculateFinalMovement();
            landingHandler.UpdateInAirTime();
            landingHandler.HandleLanding();
            gravityHandler.ApplyGravity(ref moveHandler.FinalMoveVector);
            velocityHandler.ApplySliding(ref moveHandler.FinalMoveVector);

            controller.Move(moveHandler.FinalMoveVector * Time.deltaTime);

            moveHandler.HandleHeadBob();
            moveHandler.HandleRunFOV();
            cameraController.HandleSway(velocityHandler.SmoothInputVector, input.MoveDirection.x);
        }

        public void Initialize(CharacterSettings settings)
        {
            this.settings = settings;

            controller.transform.SetLocalPositionAndRotation(LastPosition, LastRotation);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            controller.transform.SetLocalPositionAndRotation(LastPosition, LastRotation);
        }

        public void Deactivate()
        {
            LastPosition = controller.transform.localPosition;
            LastRotation = controller.transform.localRotation;
            gameObject.SetActive(false);
        }
    }
}