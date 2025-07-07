using System.Collections;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollision
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        readonly CapsuleCollider collider;
        readonly Camera camera;
        readonly Collider[] colliderBuffer;
        Coroutine resetCoroutine;
        RaycastHit groundHit;
        float lastYPosition;

        public bool JustLanded { get; private set; }
        public bool IsGrounded { get; private set; }
        public RaycastHit GroundHit
        {
            get => groundHit;
            set => groundHit = value;
        }

        public CharacterCollision(
            CharacterMovementController controller,
            CharacterData data,
            CapsuleCollider collider,
            Camera camera)
        {
            this.controller = controller;
            this.data = data;
            this.collider = collider;
            this.camera = camera;

            colliderBuffer = new Collider[8];
        }

        public bool CanMoveTo(Vector3 direction)
        {
            var adjustedGridSize = new Vector3(0.4f, 0.4f, 0.4f);
            var center = controller.transform.position +
                        (0.5f * data.gridSize * direction.normalized) +
                        Vector3.up * 0.9f; // Offset to avoid ground collisions

            var hitCount = Physics.OverlapBoxNonAlloc(
                center,
                adjustedGridSize,
                colliderBuffer,
                Quaternion.identity,
                data.obstacleLayers,
                QueryTriggerInteraction.Ignore
            );

            for (var i = 0; i < hitCount; i++)
            {
                var col = colliderBuffer[i];
                if (!col.isTrigger && col.gameObject != controller.gameObject)
                    return false; // Blocked by solid obstacle
            }

            return true;
        }

        public void UpdateGroundStatus()
        {
            var wasGrounded = IsGrounded;

            IsGrounded = Physics.SphereCast(
                controller.transform.position + Vector3.up * 0.1f,
                data.groundSphereRadius,
                Vector3.down,
                out groundHit,
                data.groundCheckDistance,
                data.groundLayers,
                QueryTriggerInteraction.Ignore
            );

            // Landing detection (only trigger if falling from a significant height)
            if (!wasGrounded && IsGrounded)
            {
                var fallDistance = lastYPosition - controller.transform.position.y;
                if (fallDistance > data.fallThreshold)
                {
                    JustLanded = true;
                    ResetJustLandedFlag();
                }
            }

            // Track Y position for fall distance calculation
            if (!IsGrounded)
                lastYPosition = controller.transform.position.y;
        }

        void ResetJustLandedFlag()
        {
            if (resetCoroutine != null)
                controller.StopCoroutine(resetCoroutine);

            resetCoroutine = controller.StartCoroutine(ResetJustLandedCoroutine());
        }

        IEnumerator ResetJustLandedCoroutine()
        {
            yield return null;
            JustLanded = false;
            resetCoroutine = null;
        }

        public bool CanStandUp()
        => !Physics.CheckSphere(
            controller.transform.localPosition + Vector3.up * (collider.height * data.crouchCameraHeightRatio + 0.1f),
            collider.radius + 0.1f, // Slightly larger than character radius
            data.ceilingCheckMask,
            QueryTriggerInteraction.Ignore
        );

        public bool HasDetectIt()
        {
            var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            var getRay = camera.ScreenPointToRay(screenCenter);

            return Physics.SphereCast(
                getRay.origin,
                data.raySphereRadius,
                getRay.direction,
                out var _,
                data.raySphereMaxDistance,
                data.characterLayer,
                QueryTriggerInteraction.Ignore
            );
        }

#if UNITY_EDITOR
        public void DrawMovementGizmos()
        {
            Gizmos.color = Color.blue;
            var direction = controller.transform.forward;
            var center = controller.transform.position +
                        (0.5f * data.gridSize * direction.normalized) +
                        Vector3.up * 0.9f;

            // Movement overlap box
            Gizmos.DrawWireCube(center, new Vector3(0.8f, 0.8f, 0.8f)); // 2x halfExtents
        }

        public void DrawGroundCheckGizmos()
        {
            // Ground sphere cast
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            var origin = controller.transform.position + Vector3.up * 0.1f;
            Gizmos.DrawWireSphere(origin, data.groundSphereRadius);
            Gizmos.DrawLine(origin, origin + Vector3.down * data.groundCheckDistance);

            // Ground hit marker
            if (IsGrounded)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(groundHit.point, 0.1f);
                Gizmos.DrawLine(groundHit.point, groundHit.point + groundHit.normal * 0.5f);
            }
        }

        public void DrawCeilingCheckGizmos()
        {
            Gizmos.color = Color.magenta;
            var ceilingCheckPos = controller.transform.position +
                Vector3.up * (collider.height * data.crouchCameraHeightRatio + 0.1f);

            Gizmos.DrawWireSphere(ceilingCheckPos, collider.radius + 0.1f);
        }

        public void DrawDetectionRayGizmos()
        {
            if (camera != null)
            {
                var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
                var ray = camera.ScreenPointToRay(screenCenter);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * data.raySphereMaxDistance);
                Gizmos.DrawWireSphere(ray.origin, data.raySphereRadius);
            }
        }
#endif
    }
}