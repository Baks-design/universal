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
        InputMovement inputMovement;
        CrouchHandler crouchHandler;
        LandingHandler landingHandler;
        JumpHandler jumpHandler;
        MoveHandler moveHandler;
        MovementCalculator movementCalculator;
        HeadBobHandler headBobHandler;

        public CharacterSettings Settings => settings;
        public Transform CharacterTransform => controller.transform;
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public bool IsMoving => controller.velocity.sqrMagnitude > 0.1f;
        public bool IsRunning => inputMovement.IsRunning;
        public Vector3 Direction => movementCalculator.FinalMoveDirection;

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
            jumpHandler = new JumpHandler(movementSettings, collisionController, crouchHandler);
            movementCalculator = new MovementCalculator(movementSettings, collisionController, crouchHandler, controller);
            moveHandler = new MoveHandler(controller, yawTransform, movementSettings, collisionController,
                movementCalculator, headBobHandler, cameraController, crouchHandler);
        }

        void OnEnable()
        {
            input.Move += OnGetMove;
            input.Run += OnGetRun;
            input.Crouch += OnGetCrouch;
            input.Jump += OnGetJump;
        }

        void OnDisable()
        {
            input.Move -= OnGetMove;
            input.Run -= OnGetRun;
            input.Crouch -= OnGetCrouch;
            input.Jump -= OnGetJump;
        }

        void OnGetMove(Vector2 value) => inputMovement.Move = value;

        void OnGetRun(bool value) => inputMovement.IsRunning = value;

        void OnGetCrouch() => crouchHandler.HandleCrouch();

        void OnGetJump() => jumpHandler.TryJump(ref moveHandler.FinalMoveVector);

        void Update()
        {
            moveHandler.RotateTowardsCamera();

            movementCalculator.UpdateMovement(inputMovement);

            moveHandler.SmoothMovementDirection();
            moveHandler.CalculateFinalMovement();
            landingHandler.UpdateInAirTime();
            landingHandler.HandleLanding();
            jumpHandler.ApplyGravity(ref moveHandler.FinalMoveVector);

            controller.Move(moveHandler.FinalMoveVector * Time.deltaTime);

            moveHandler.HandleHeadBob(inputMovement); 
            moveHandler.HandleRunFOV(inputMovement);
            cameraController.HandleSway(movementCalculator.SmoothInputVector, inputMovement.Move.x);
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

//TODO: Fix Headbob
//TODO: Fix Direction
//TODO: Fix Atributtes Values
//TODO: Fix Inputs(Jump, Crouch, Run)
//TODO: Fix Spawn 
//TODO: Fix Camera Rotation 