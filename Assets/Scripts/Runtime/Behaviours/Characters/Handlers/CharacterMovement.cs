using System.Collections;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovement
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        readonly Rigidbody rigidbody;
        Coroutine movementCoroutine;

        public Vector3 TargetPosition { get; set; }
        public bool IsMoving { get; set; }
        public bool IsRunning { get; set; }
        public bool JustLanded { get; set; }

        public CharacterMovement(
            CharacterMovementController controller,
            CharacterData data,
            Rigidbody rigidbody)
        {
            this.controller = controller;
            this.data = data;
            this.rigidbody = rigidbody;

            TargetPosition = controller.transform.position;
            IsMoving = false;
            IsRunning = false;
            JustLanded = false;
        }

        public void MoveInDirection(Vector3 direction)
        {
            if (IsMoving || controller.CharacterRotation.IsRotating) return;

            if (controller.CharacterCollision.CanMoveTo(direction))
            {
                TargetPosition = controller.CharacterCollision.SnapToGrid(
                    controller.transform.position + direction * data.gridSize);
                IsMoving = true;
            }
        }

        public void MoveToTargetHandle()
        {
            if (!IsMoving) return;

            if (movementCoroutine != null)
                controller.StopCoroutine(movementCoroutine);

            movementCoroutine = controller.StartCoroutine(MoveToTarget());
        }

        IEnumerator MoveToTarget()
        {
            var startPosition = controller.transform.position;
            var moveProgress = 0f;

            // Calculate move direction once
            var moveDirection = (TargetPosition - startPosition).normalized;

            while (moveProgress < 1f)
            {
                // Check for obstacles during movement
                if (Physics.CheckSphere(controller.transform.position + moveDirection * 0.5f, 0.3f, data.obstacleLayers))
                {
                    // Abort movement if something blocks the path
                    TargetPosition = controller.transform.position;
                    break;
                }

                // Calculate progress (0 to 1)
                moveProgress = Mathf.Clamp01(Vector3.Distance(startPosition, controller.transform.position) / data.gridSize);

                // Smooth movement using easing
                var easedProgress = Helpers.EaseInOutQuad(moveProgress);

                // Physics-based movement
                rigidbody.MovePosition(Vector3.Lerp(startPosition, TargetPosition, easedProgress));

                // Adjust progress slightly faster near the end to ensure completion
                if (moveProgress > 0.9f) moveProgress += Time.deltaTime * 2f;

                yield return null;
            }

            // Final snap to grid position
            controller.transform.position = TargetPosition;
            rigidbody.linearVelocity = Vector3.zero;
            IsMoving = false;
        }
    }
}