using UnityEngine;
using Universal.Runtime.Components.Input;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly PlayerController controller;
        readonly Transform character;
        readonly CharacterData data;
        readonly Camera camera;
        readonly IMovementInputReader movementInput;
        Vector3Int queuedDirection;
        Vector3 startPosition;
        Vector3 targetPosition;
        const float MOVEMENT_THRESHOLD = 0.01f;
        const float DIRECTION_DEADZONE = 0.1f;
        float moveProgress;
        bool hasMovementInput;

        public Vector3 FacingDirection { get; set; } 
        public Vector2 InputBuffer { get; private set; } 
        public bool IsMoving { get; set; } 
        public bool IsBlocked { get; private set; } 

        public CharacterMovement(
            PlayerController controller,
            Transform character,
            CharacterData data,
            Camera camera,
            IMovementInputReader movementInput)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;
            this.camera = camera;
            this.movementInput = movementInput;

            startPosition = targetPosition = character.position;
        }

        public void HandleMovementInput()
        {
            // Read and buffer input
            var currentInput = movementInput.MoveDirection;
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

        Vector3Int GetGridAlignedDirection(Vector3 worldDirection)
        {
            if (worldDirection.sqrMagnitude < DIRECTION_DEADZONE)
                return Vector3Int.zero;

            var xAbs = Abs(worldDirection.x);
            var zAbs = Abs(worldDirection.z);

            return xAbs > zAbs ?
                (worldDirection.x > 0f ? Vector3Int.right : Vector3Int.left) :
                (worldDirection.z > 0f ? Vector3Int.up : Vector3Int.down);
        }

        void HandleMovementQueuing()
        {
            if (queuedDirection == Vector3Int.zero) return;

            var nextCell = controller.CurrentGridPosition + queuedDirection;
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

        bool IsCellBlocked(Vector3Int cell)
        {
            var worldPosition = controller.Grid.GetCellCenterWorld(cell);
            return Physics.CheckSphere(worldPosition, data.obstacleCheckRadius, data.obstacleMask);
        }

        void StartMovement(Vector3Int targetCell)
        {
            startPosition = character.position;
            targetPosition = controller.Grid.GetCellCenterWorld(targetCell);

            if (Vector3.Distance(startPosition, targetPosition) < MOVEMENT_THRESHOLD)
            {
                character.position = targetPosition;
                controller.CurrentGridPosition = targetCell;
                return;
            }

            moveProgress = 0f;
            IsMoving = true;
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
            {
                character.position = targetPosition;
                controller.CurrentGridPosition = controller.Grid.WorldToCell(targetPosition);
                IsMoving = false;
            }
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