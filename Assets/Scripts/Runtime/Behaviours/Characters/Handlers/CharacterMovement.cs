using UnityEngine;
using Universal.Runtime.Components.Input;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterMovementController movement;
        readonly Transform character;
        readonly CharacterData data;
        readonly Grid grid;
        readonly Camera camera;
        readonly IPlayerInputReader inputReader;
        static readonly Vector3Int Right = Vector3Int.right;
        static readonly Vector3Int Left = Vector3Int.left;
        static readonly Vector3Int Up = Vector3Int.up;
        static readonly Vector3Int Down = Vector3Int.down;
        const float MOVEMENT_THRESHOLD = 0.01f;
        const float DIRECTION_DEADZONE = 0.1f;
        Vector3 startPosition, targetPosition;
        Vector3Int queuedDirection;
        bool hasMovementInput;
        float moveProgress;

        public bool IsMoving { get; set; }
        public Vector3 FacingDirection { get; set; }
        public Vector2 InputBuffer { get; private set; }
        public bool IsBlocked { get; private set; }
        public bool IsRunning { get; private set; }

        public CharacterMovement(
            CharacterMovementController movement,
            Transform character,
            CharacterData data,
            Grid grid,
            Camera camera,
            IPlayerInputReader inputReader)
        {
            this.movement = movement;
            this.character = character;
            this.data = data;
            this.grid = grid;
            this.camera = camera;
            this.inputReader = inputReader;

            ResetMovementState();
        }

        void ResetMovementState()
        {
            startPosition = targetPosition = character.position;
            moveProgress = 0f;
            IsMoving = false;
            InputBuffer = Vector2.zero;
            queuedDirection = Vector3Int.zero;
        }

        public void SnapToGrid()
        {
            movement.CurrentGridPosition = grid.WorldToCell(character.position);
            character.position = grid.GetCellCenterWorld(movement.CurrentGridPosition);
            FacingDirection = character.forward;
        }

        public void HandleMovementInput()
        {
            if (movement.CharacterRotation.IsRotating) return;

            // Read and buffer input
            var currentInput = inputReader.MoveDirection;
            hasMovementInput = currentInput.sqrMagnitude > 0.1f;

            UpdateInputBuffer(currentInput);

            // Convert to world direction
            var worldDirection = GetWorldSpaceDirection(InputBuffer);
            if (worldDirection.sqrMagnitude > DIRECTION_DEADZONE)
            {
                queuedDirection = GetGridAlignedDirection(worldDirection);
                FacingDirection = new Vector3(queuedDirection.x, 0f, queuedDirection.z).normalized;
            }

            HandleMovementQueuing();
        }

        void UpdateInputBuffer(Vector2 currentInput)
        {
            if (hasMovementInput)
            {
                InputBuffer = Vector2.Lerp(InputBuffer, currentInput, Time.deltaTime * data.inputResponseSpeed);
                InputBuffer = Vector2.ClampMagnitude(InputBuffer, 1f);
            }
            else
                InputBuffer = Vector2.Lerp(InputBuffer, Vector2.zero, Time.deltaTime * data.inputDeceleration);
        }

        void HandleMovementQueuing()
        {
            if (queuedDirection == Vector3Int.zero) return;

            var nextCell = movement.CurrentGridPosition + queuedDirection;
            var cellBlocked = IsCellBlocked(nextCell);

            // Queue next movement if current one is almost complete
            if (IsMoving && moveProgress >= data.moveQueueThreshold && !cellBlocked)
            {
                StartMovement(nextCell);
                queuedDirection = Vector3Int.zero;
            }
            // Start new movement if not moving
            else if (!IsMoving)
            {
                if (!cellBlocked)
                {
                    IsBlocked = false;
                    StartMovement(nextCell);
                    queuedDirection = Vector3Int.zero;
                }
                else
                    IsBlocked = true;
            }
        }

        public void UpdatePosition()
        {
            if (!IsMoving) return;

            moveProgress += Time.deltaTime / Max(0.0001f, data.moveDuration);
            moveProgress = Clamp01(moveProgress);

            // Smooth movement with acceleration/deceleration
            var smoothedProgress = data.moveCurve?.Evaluate(moveProgress) ?? moveProgress;
            character.position = Vector3.Lerp(startPosition, targetPosition, smoothedProgress);

            if (moveProgress >= 1f)
                CompleteMovement();
        }

        void CompleteMovement()
        {
            character.position = targetPosition;
            movement.CurrentGridPosition = grid.WorldToCell(targetPosition);
            IsMoving = false;
        }

        Vector3 GetWorldSpaceDirection(Vector2 input)
        {
            if (input.sqrMagnitude < DIRECTION_DEADZONE)
                return Vector3.zero;

            var cameraForward = camera != null ?
                Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized :
                Vector3.forward;

            var cameraRight = camera != null ?
                Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized :
                Vector3.right;

            return (cameraForward * input.y) + (cameraRight * input.x);
        }

        void StartMovement(Vector3Int targetCell)
        {
            startPosition = character.position;
            targetPosition = grid.GetCellCenterWorld(targetCell);

            if (Vector3.Distance(startPosition, targetPosition) < MOVEMENT_THRESHOLD)
            {
                character.position = targetPosition;
                movement.CurrentGridPosition = targetCell;
                return;
            }

            moveProgress = 0f;
            IsMoving = true;
        }

        Vector3Int GetGridAlignedDirection(Vector3 worldDirection)
        {
            if (worldDirection.sqrMagnitude < DIRECTION_DEADZONE)
                return Vector3Int.zero;

            var xAbs = Abs(worldDirection.x);
            var zAbs = Abs(worldDirection.z);

            return xAbs > zAbs ?
                (worldDirection.x > 0f ? Right : Left) :
                (worldDirection.z > 0f ? Up : Down);
        }

        bool IsCellBlocked(Vector3Int cell)
        {
            var worldPosition = grid.GetCellCenterWorld(cell);
            return Physics.CheckSphere(worldPosition, data.obstacleCheckRadius, data.obstacleMask);
        }

#if UNITY_EDITOR
        public void DrawMovementGizmos()
        {
            // Draw movement path
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPosition, targetPosition);
            // Draw target position
            Gizmos.color = IsBlocked ? Color.red : Color.blue;
            Gizmos.DrawWireCube(targetPosition, Vector3.one * 0.8f);
        }
#endif
    }
}