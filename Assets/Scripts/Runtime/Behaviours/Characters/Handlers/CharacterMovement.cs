using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterMovementController movement;
        readonly Transform character;
        readonly CharacterData data;
        readonly Grid grid;
        readonly Camera camera;
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
            Camera camera)
        {
            this.movement = movement;
            this.character = character;
            this.data = data;
            this.grid = grid;
            this.camera = camera;

            startPosition = targetPosition = character.position;
            moveProgress = 0f;
            IsMoving = false;
            FacingDirection = Vector3.forward;
            InputBuffer = Vector2.zero;
        }

        public void HandleMovementInput()
        {
            // Read and buffer input
            var currentInput = PlayerMapInputProvider.Move.ReadValue<Vector2>();
            hasMovementInput = currentInput.sqrMagnitude > 0.1f;
            if (hasMovementInput)
            {
                InputBuffer = Vector2.Lerp(InputBuffer, currentInput, Time.deltaTime * data.inputResponseSpeed);
                InputBuffer = Vector2.ClampMagnitude(InputBuffer, 1f);
            }
            else
                InputBuffer = Vector2.Lerp(InputBuffer, Vector2.zero, Time.deltaTime * data.inputDeceleration);

            // Convert to world direction
            var worldDirection = GetWorldSpaceDirection(InputBuffer);
            if (worldDirection.sqrMagnitude > 0.01f)
            {
                queuedDirection = GetGridAlignedDirection(worldDirection);
                FacingDirection = new Vector3(queuedDirection.x, 0f, queuedDirection.z).normalized;
            }

            // Queue next movement if current one is almost complete
            if (IsMoving && moveProgress >= data.moveQueueThreshold && queuedDirection != Vector3Int.zero)
            {
                var nextCell = movement.CurrentGridPosition + queuedDirection;
                if (!IsCellBlocked(nextCell))
                {
                    StartMovement(nextCell);
                    queuedDirection = Vector3Int.zero;
                }
            }
            // Start new movement if not moving
            else if (!IsMoving && queuedDirection != Vector3Int.zero)
            {
                var nextCell = movement.CurrentGridPosition + queuedDirection;
                if (!IsCellBlocked(nextCell))
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

            moveProgress += Time.deltaTime / Mathf.Max(0.0001f, data.moveDuration);
            moveProgress = Mathf.Clamp01(moveProgress);

            // Smooth movement with acceleration/deceleration
            var smoothedProgress = data.moveCurve?.Evaluate(moveProgress) ?? moveProgress;
            character.position = Vector3.Lerp(startPosition, targetPosition, smoothedProgress);

            if (moveProgress >= 1f)
            {
                character.position = targetPosition;
                movement.CurrentGridPosition = grid.WorldToCell(targetPosition);
                IsMoving = false;
            }
        }

        Vector3 GetWorldSpaceDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f) return Vector3.zero;

            var cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            var cameraRight = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;

            return (cameraForward * input.y) + (cameraRight * input.x);
        }

        void StartMovement(Vector3Int targetCell)
        {
            startPosition = character.position;
            targetPosition = grid.GetCellCenterWorld(targetCell);

            // Immediately start if very close to target
            if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
            {
                character.position = targetPosition;
                movement.CurrentGridPosition = targetCell;
                IsMoving = false;
                return;
            }

            moveProgress = 0f;
            IsMoving = true;
        }

        Vector3Int GetGridAlignedDirection(Vector3 worldDirection)
        {
            if (worldDirection.sqrMagnitude < 0.1f) return Vector3Int.zero;

            // Apply hysteresis to prevent direction flips
            var xAbs = Mathf.Abs(worldDirection.x);
            var zAbs = Mathf.Abs(worldDirection.z);
            var threshold = 0.3f;
            if (xAbs > threshold || zAbs > threshold)
                return xAbs > zAbs
                    ? worldDirection.x > 0f ? Vector3Int.right : Vector3Int.left
                    : worldDirection.z > 0f ? Vector3Int.up : Vector3Int.down;
            return Vector3Int.zero;
        }

        bool IsCellBlocked(Vector3Int cell)
        {
            var worldPosition = grid.GetCellCenterWorld(cell);
            return Physics.CheckSphere(worldPosition, data.obstacleCheckRadius, data.obstacleMask);
        }
    }
}