using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour, IGridMover
    {
        [SerializeField, Self] CharacterCollisionController collision;
        [SerializeField, Self] MovementCommandQueue commandQueue;
        [SerializeField, Self] Transform tr;
        [SerializeField, InlineEditor] CharacterData data;
        readonly TurnRightCommand turnRightCommand = new();
        readonly TurnLeftCommand turnLeftCommand = new();
        readonly MoveForwardCommand moveForwardCommand = new();
        readonly MoveBackwardCommand moveBackwardCommand = new();
        readonly StrafeRightCommand strafeRightCommand = new();
        readonly StrafeLeftCommand strafeLeftCommand = new();
        IMovementAnimator movement;
        IMovementAnimator rotation;
        IMovementInputReader input;

        public float GridSize => data.gridSize;
        public bool IsAnimating { get; private set; }
        public bool IsMoving => movement.IsAnimating;
        public bool IsRotating => rotation.IsAnimating;
        public Vector3 Position { get => tr.localPosition; set => tr.localPosition = value; }
        public Quaternion Rotation { get => tr.rotation; set => tr.rotation = value; }
        public CharacterCollisionController Collision => collision;

        void Awake()
        {
            ServiceLocator.Global.Get(out input);

            movement = new CurveBasedMovementAnimator(data);
            rotation = new QuaternionRotationAnimator(data);

            movement.AnimateMovement(tr.localPosition, tr.localPosition, 0f);
            rotation.AnimateRotation(tr.localRotation, tr.localRotation, 0f);
        }

        void OnEnable()
        {
            input.TurnRight += TurnRightCommand;
            input.TurnLeft += TurnLeftCommand;
            input.Move += MoveCommand;
            input.Run += RunForwardPress;
        }

        void OnDisable()
        {
            input.TurnRight -= TurnRightCommand;
            input.TurnLeft -= TurnLeftCommand;
            input.Move -= MoveCommand;
            input.Run -= RunForwardPress;
        }

        void TurnRightCommand() => TryExecuteCommand(turnRightCommand);

        void TurnLeftCommand() => TryExecuteCommand(turnLeftCommand);

        void MoveCommand(Vector2 move)
        {
            if (move.y > 0f) TryExecuteCommand(moveForwardCommand);
            if (move.y < 0f) TryExecuteCommand(moveBackwardCommand);
            if (move.x > 0f) TryExecuteCommand(strafeRightCommand);
            if (move.x < 0f) TryExecuteCommand(strafeLeftCommand);
        }

        void RunForwardPress(bool value)
        {
            if (value)
                commandQueue.HandleForwardPress();
            else
                commandQueue.HandleForwardRelease();
        }

        public bool TryExecuteCommand(IMovementCommand command)
        {
            if (!command.CanExecute(this)) return false;
            switch (command)
            {
                case GridMovementCommand gridCommand:
                    var targetPos = tr.localPosition + gridCommand.GetMovementDirection(this) * data.gridSize;
                    if (!Collision.IsPositionFree(targetPos)) return false;
                    IsAnimating = movement.IsAnimating;
                    movement.AnimateMovement(tr.localPosition, targetPos, data.moveDuration);
                    break;
                case GridTurnCommand turnCommand:
                    var targetRot = tr.localRotation * Quaternion.Euler(0f, turnCommand.TurnAngle, 0f);
                    rotation.AnimateRotation(tr.localRotation, targetRot, data.rotationDuration);
                    break;
            }
            command.Execute(this);
            return true;
        }

        void Update()
        {
            movement.UpdateAnimation();
            rotation.UpdateAnimation();
            tr.SetLocalPositionAndRotation(movement.CurrentPosition, rotation.CurrentRotation);
        }
    }
}