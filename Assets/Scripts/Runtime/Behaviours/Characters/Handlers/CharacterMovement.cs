using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterController character;
        readonly CharacterData data;
        readonly Grid grid;
        readonly Camera camera;
        Vector3Int currentCell;
        Vector3 startPosition;
        Vector3 targetPosition;
        float moveProgress;
        float lastInputTime;

        public bool IsMoving { get; set; } 
        public Vector3Int CurrentGridPosition => currentCell;
        public Vector3 FacingDirection { get; private set; }

        public CharacterMovement(
            CharacterController character,
            CharacterData data,
            Grid grid,
            Camera camera)
        {
            this.character = character;
            this.data = data;
            this.grid = grid;
            this.camera = camera;

            IsMoving = false;
            SnapToGrid();
        }

        public void SnapToGrid()
        {
            currentCell = grid.WorldToCell(character.transform.position);
            character.transform.position = grid.GetCellCenterWorld(currentCell);
            FacingDirection = character.transform.forward;
        }

        public void UpdateMovement()
        {
            if (IsMoving)
            {
                moveProgress += Time.deltaTime / data.moveDuration;
                var t = data.moveCurve.Evaluate(moveProgress);
                character.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                if (moveProgress >= 1f)
                {
                    character.transform.position = targetPosition;
                    currentCell = grid.WorldToCell(targetPosition);
                    IsMoving = false;
                }
            }
        }

        public void HandleGridMovement()
        {
            if (IsMoving || Time.time < lastInputTime + data.inputCooldown)
                return;

            var worldDirection = GetWorldSpaceDirection();
            if (worldDirection == Vector3.zero) return;

            var gridDirection = GetGridAlignedDirection(worldDirection);
            if (gridDirection == Vector3Int.zero) return;

            var targetCell = currentCell + gridDirection;
            if (!IsCellBlocked(targetCell))
            {
                StartMovement(targetCell);
                lastInputTime = Time.time;
                FacingDirection = new Vector3(gridDirection.x, 0, gridDirection.y).normalized;
            }
        }

        void StartMovement(Vector3Int targetCell)
        {
            startPosition = character.transform.position;
            targetPosition = grid.GetCellCenterWorld(targetCell);
            moveProgress = 0f;
            IsMoving = true;
        }

        public void CancelMovement()
        {
            IsMoving = false;
            SnapToGrid();
        }

        Vector3 GetWorldSpaceDirection()
        {
            var moveInput = PlayerMapInputProvider.Move.ReadValue<Vector2>();
            Debug.Log($"Raw Input: {moveInput}");
            if (moveInput.magnitude < data.inputDeadzone)
            {
                Debug.Log("Input below deadzone threshold");
                return Vector3.zero;
            }
            // Normalize input if it exceeds 1 magnitude (diagonal movement)
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            // Get camera-relative directions
            var cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            var cameraRight = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;

            return (cameraForward * moveInput.y) + (cameraRight * moveInput.x);
        }

        Vector3Int GetGridAlignedDirection(Vector3 worldDirection)
        {
            if (worldDirection.sqrMagnitude < 0.01f)
                return Vector3Int.zero;

            // Get absolute values for comparison
            var absX = Mathf.Abs(worldDirection.x);
            var absZ = Mathf.Abs(worldDirection.z);
            // Apply threshold to prevent accidental diagonal movement
            if (Mathf.Abs(absX - absZ) < data.diagonalThreshold)
            {
                Debug.Log("Diagonal input detected - movement blocked");
                return Vector3Int.zero;
            }

            // Determine primary movement direction
            if (absX > absZ)
                return worldDirection.x > 0 ? Vector3Int.right : Vector3Int.left;
            else
                return worldDirection.z > 0 ? new Vector3Int(0, 1, 0) : new Vector3Int(0, -1, 0);
        }

        bool IsCellBlocked(Vector3Int cell)
        {
            var worldPosition = grid.GetCellCenterWorld(cell);
            var isBlocked = Physics.CheckSphere(worldPosition, data.obstacleCheckRadius, data.obstacleMask);
            if (isBlocked)
                Debug.DrawLine(character.transform.position, worldPosition, Color.red, 1f);
            return isBlocked;
        }
    }
}