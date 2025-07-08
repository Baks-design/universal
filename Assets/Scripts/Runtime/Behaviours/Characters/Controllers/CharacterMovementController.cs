using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour, IGridMover
    {
        [SerializeField, Self] CharacterPlayerController controller;
        [SerializeField, Self] MovementCommandQueue movementCommandQueue;
        [SerializeField, Self] Transform tr;
        [SerializeField, InlineEditor] CharacterData data;
        readonly TurnRightCommand turnRightCommand = new();
        readonly TurnLeftCommand turnLeftCommand = new();
        readonly MoveForwardCommand moveForwardCommand = new();
        readonly MoveBackwardCommand moveBackwardCommand = new();
        readonly StrafeRightCommand strafeRightCommand = new();
        readonly StrafeLeftCommand strafeLeftCommand = new();
        IMovementAnimator movementAnimator;
        IMovementAnimator rotationAnimator;
        IMovementInputReader movementInput;
         CollisionChecker collisionChecker;

        public float GridSize => data.gridSize;
        public bool IsMoving => movementAnimator.IsAnimating;
        public bool IsRotating => rotationAnimator.IsAnimating;
        public Vector3 Position
        {
            get => tr.position;
            set => tr.position = value;
        }
        public Quaternion Rotation
        {
            get => tr.rotation;
            set => tr.rotation = value;
        }
        public CollisionChecker CollisionChecker => collisionChecker;

        void Awake()
        {
            ServiceLocator.Global.Get(out movementInput);
            collisionChecker = new CollisionChecker(data, tr, Camera.main);
            movementAnimator = new CurveBasedMovementAnimator(data);
            rotationAnimator = new QuaternionRotationAnimator(data);
        }

        void OnEnable()
        {
            movementInput.Move += MoveCommand;
            movementInput.TurnRight += TurnRightCommand;
            movementInput.TurnLeft += TurnLeftCommand;
            movementInput.TurnLeft += RunForwardPress;
            movementInput.TurnLeft += RunForwardRelease;
        }

        void OnDisable()
        {
            movementInput.Move -= MoveCommand;
            movementInput.TurnRight -= TurnRightCommand;
            movementInput.TurnLeft -= TurnLeftCommand;
            movementInput.TurnLeft -= RunForwardPress;
            movementInput.TurnLeft -= RunForwardRelease;
        }

        void TurnRightCommand()
        {
            if (!controller.IsCurrentStateEqual(controller.MovementState)) return;
            TryExecuteCommand(turnRightCommand);
        }

        void TurnLeftCommand()
        {
            if (!controller.IsCurrentStateEqual(controller.MovementState)) return;
            TryExecuteCommand(turnLeftCommand);
        }

        void MoveCommand(Vector2 move)
        {
            if (!controller.IsCurrentStateEqual(controller.MovementState)) return;
            if (move.y > 0f) TryExecuteCommand(moveForwardCommand);
            if (move.y < 0f) TryExecuteCommand(moveBackwardCommand);
            if (move.x > 0f) TryExecuteCommand(strafeRightCommand);
            if (move.x < 0f) TryExecuteCommand(strafeLeftCommand);
        }

        public bool TryExecuteCommand(IMovementCommand command)
        {
            if (!command.CanExecute(this)) return false;
            switch (command)
            {
                case GridMovementCommand gridCmd:
                    var targetPos = Position + gridCmd.GetMovementDirection(this) * GridSize;
                    if (CollisionChecker.IsPositionFree(targetPos))
                        movementAnimator.AnimateMovement(Position, targetPos, 1f / data.movementSpeed);
                    break;
                case TurnRightCommand _:
                    rotationAnimator.AnimateRotation(Rotation, Rotation, 1f / data.rotationSpeed);
                    break;
                case TurnLeftCommand _:
                    rotationAnimator.AnimateRotation(Rotation, Rotation, 1f / data.rotationSpeed);
                    break;
            }

            command.Execute(this);
            return true;
        }

        public void RunForwardPress() => movementCommandQueue.HandleForwardPress();

        public void RunForwardRelease() => movementCommandQueue.HandleForwardRelease();

        void Update()
        {
            CollisionChecker.UpdataCharacterDetect();
            CollisionChecker.UpdateGroundDetect();

            movementAnimator.UpdateAnimation();
            rotationAnimator.UpdateAnimation();
            tr.SetPositionAndRotation(
                movementAnimator.CurrentPosition,
                rotationAnimator.CurrentRotation
            );
        }
    }
}