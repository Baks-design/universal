using System.Collections;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollision
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        readonly Collider[] colliderBuffer = new Collider[8];
        bool isGrounded;
        bool justLanded;
        float lastYPosition;
        RaycastHit groundHit;
        Coroutine resetCoroutine;

        public bool IsGrounded => isGrounded;
        public bool JustLanded => justLanded;
        public float LastYPosition => lastYPosition;
        public RaycastHit GroundHit => groundHit;

        public CharacterCollision(
            CharacterMovementController controller,
            CharacterData data)
        {
            this.controller = controller;
            this.data = data;
        }

        public bool CanMoveTo(Vector3 direction)
        {
            Vector3 halfExtents = default;
            halfExtents.x = 0.4f;
            halfExtents.y = 0.9f;
            halfExtents.z = 0.4f;

            var center = controller.transform.position + 0.5f * data.gridSize * direction + Vector3.up * 0.9f;

            var hitCount = Physics.OverlapBoxNonAlloc(
                center,
                halfExtents,
                colliderBuffer,
                Quaternion.identity
            );

            for (var i = 0; i < hitCount; i++)
                if (!colliderBuffer[i].isTrigger &&
                    colliderBuffer[i].gameObject != controller.gameObject)
                    return false;

            return true;
        }

        public Vector3 SnapToGrid(Vector3 position)
        => new(
            Round(position.x / data.gridSize) * data.gridSize,
            position.y,
            Round(position.z / data.gridSize) * data.gridSize
        );

        public void CollisionEnter()
        {
            if (!controller.CharacterMovement.IsMoving) return;

            controller.CharacterMovement.IsMoving = false;
            controller.CharacterMovement.TargetPosition = controller.transform.position;
        }

        public void UpdateGroundStatus()
        {
            var wasGrounded = isGrounded;
            isGrounded = Physics.SphereCast(
                controller.transform.transform.position + Vector3.up * 0.1f,
                data.groundSphereRadius,
                Vector3.down,
                out groundHit,
                data.groundCheckDistance,
                data.groundLayers
            );

            // Landing detection
            if (!wasGrounded && isGrounded)
            {
                var fallDistance = lastYPosition - controller.transform.position.y;
                if (fallDistance > data.fallThreshold)
                {
                    justLanded = true;

                    // Automatically reset after one frame
                    if (resetCoroutine != null)
                        controller.StopCoroutine(resetCoroutine);
                    resetCoroutine = controller.StartCoroutine(ResetJustLanded());
                }
            }

            // Track Y position for fall distance calculation
            if (!isGrounded)
                lastYPosition = controller.transform.position.y;
        }

        IEnumerator ResetJustLanded()
        {
            yield return null; // Wait one frame
            justLanded = false;
        }
    }
}