using System.Collections;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        Coroutine movementCoroutine;

        public Vector3 TargetPosition { get; private set; }
        public bool IsMoving { get; private set; }

        public CharacterMovement(CharacterMovementController controller, CharacterData data)
        {
            this.controller = controller;
            this.data = data;

            TargetPosition = controller.transform.localPosition;
            IsMoving = false;
        }

        public void MoveInDirection(Vector3 direction)
        {
            if (IsMoving ||
                controller.CharacterRotation.IsRotating ||
                controller.CharacterCrouch.IsCrouching) return;

            var dir = direction.normalized;
            if (controller.CharacterCollision.CanMoveTo(dir))
            {
                TargetPosition = SnapToGrid(controller.transform.localPosition + dir * data.gridSize);

                if (movementCoroutine != null)
                    controller.StopCoroutine(movementCoroutine);
                movementCoroutine = controller.StartCoroutine(MoveToTarget());

                IsMoving = true;
            }
        }

        Vector3 SnapToGrid(Vector3 position)
        {
            var gridSize = data.gridSize;
            return new Vector3(
                Round(position.x / gridSize) * gridSize,
                position.y, // Preserve Y for slopes/stairs
                Round(position.z / gridSize) * gridSize
            );
        }

        IEnumerator MoveToTarget()
        {
            var startPosition = controller.transform.localPosition;
            var distance = Vector3.Distance(startPosition, TargetPosition);
            var elapsedTime = 0f;
            var moveDuration = distance / data.moveSpeed;

            while (elapsedTime < moveDuration)
            {
                // Smooth lerp movement
                controller.transform.localPosition = Vector3.Lerp(
                    startPosition,
                    TargetPosition,
                    elapsedTime / moveDuration
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Finalize movement
            controller.transform.localPosition = TargetPosition; // Snap to grid
            IsMoving = false;
        }
    }
}