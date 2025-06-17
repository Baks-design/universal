using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterMovementController movement;
        readonly Transform character;
        readonly CharacterData data;
        readonly Grid grid;
        readonly Camera camera;
        Vector3 startPosition;
        Vector3 targetPosition;
        float moveProgress;
        float lastInputTime;

        public bool IsMoving { get; set; }
        public bool IsRunHold { get; private set; }
        public Vector3 FacingDirection { get; set; }

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

            startPosition = Vector3.zero;
            targetPosition = Vector3.zero;
            moveProgress = 0f;
            lastInputTime = 0f;
            IsMoving = false;
        }

        public void UpdateMovement()
        {
            if (!IsMoving)
                return;

            moveProgress += Time.deltaTime / data.moveDuration;
            var t = data.moveCurve.Evaluate(moveProgress);
            character.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (moveProgress >= 1f)
            {
                character.position = targetPosition;
                movement.CurrentGridPosition = grid.WorldToCell(targetPosition);
                IsMoving = false;
            }
        }

        public void HandleGridMovement()
        {
            if (IsMoving || Time.time < lastInputTime + data.inputCooldown)
                return;

            var worldDirection = GetWorldSpaceDirection();
            if (worldDirection == Vector3.zero)
                return;

            var gridDirection = GetGridAlignedDirection(worldDirection);
            if (gridDirection == Vector3Int.zero)
                return;

            var targetCell = movement.CurrentGridPosition + gridDirection;
            if (!IsCellBlocked(targetCell))
            {
                StartMovement(targetCell);
                lastInputTime = Time.time;
                FacingDirection = new Vector3(gridDirection.x, 0f, gridDirection.y).normalized;
            }
        }

        void StartMovement(Vector3Int targetCell)
        {
            startPosition = character.position;
            targetPosition = grid.GetCellCenterWorld(targetCell);
            moveProgress = 0f;
            IsMoving = true;
        }

        Vector3 GetWorldSpaceDirection()
        {
            var moveInput = PlayerMapInputProvider.Move.ReadValue<Vector2>();

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
            // Determine primary movement direction
            if (absX > absZ)
                return worldDirection.x > 0f ? AMathfs.SetDirection(4) : AMathfs.SetDirection(3);
            else
                return worldDirection.z > 0f ? AMathfs.SetDirection(1) : AMathfs.SetDirection(2);
        }

        bool IsCellBlocked(Vector3Int cell)
        {
            var worldPosition = grid.GetCellCenterWorld(cell);
            return Physics.CheckSphere(worldPosition, data.obstacleCheckRadius, data.obstacleMask);
        }
    }
}