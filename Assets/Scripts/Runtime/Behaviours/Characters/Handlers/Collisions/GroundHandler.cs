using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class GroundHandler
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        RaycastHit hitInfo;

        public bool IsGrounded { get; set; }
        public bool JustLanded { get; private set; }
        public bool IsOnSteepSlope { get; private set; }
        public float SlopeAngle { get; private set; }
        public RaycastHit HitInfo => hitInfo;

        public GroundHandler(CharacterController controller, PhysicsSettings settings)
        {
            this.controller = controller;
            this.settings = settings;
        }

        public void CheckGround()
        {
            var wasGrounded = IsGrounded;

            var radius = controller.radius;
            var height = controller.height;

            var center = controller.transform.localPosition + controller.center;
            var point1 = center + controller.transform.up * (height / 2f - radius);
            var point2 = center - controller.transform.up * (height / 2f - radius);

            IsGrounded = Physics.CapsuleCast(
                point1,
                point2,
                radius,
                -controller.transform.up,
                out hitInfo,
                settings.rayGroundLength + controller.skinWidth,
                settings.groundLayers);

            if (IsGrounded)
            {
                SlopeAngle = Vector3.Angle(hitInfo.normal, controller.transform.up);
                IsOnSteepSlope = SlopeAngle > controller.slopeLimit;
            }
            else
            {
                SlopeAngle = 0f;
                IsOnSteepSlope = false;
            }

            JustLanded = !wasGrounded && IsGrounded;
        }

        public Vector3 GetSlopeSlideDirection()
        {
            if (!IsOnSteepSlope) return Vector3.zero;

            return Vector3.ProjectOnPlane(-controller.transform.up, hitInfo.normal).normalized;
        }

        public void DrawGroundCheckGizmo()
        {
            Gizmos.color = IsGrounded ? (IsOnSteepSlope ? Color.yellow : Color.green) : Color.red;

            var radius = controller.radius;
            var height = controller.height;
            var center = controller.transform.localPosition + controller.center;

            var point1 = center + controller.transform.up * (height / 2f - radius);
            var point2 = center - controller.transform.up * (height / 2f - radius);

            DrawCapsuleGizmo(point1, point2, radius);

            Gizmos.DrawLine(
                center,
                center + -controller.transform.up * (settings.rayGroundLength + controller.skinWidth)
            );

            var endPoint = center + -controller.transform.up * (settings.rayGroundLength + controller.skinWidth);
            DrawCapsuleGizmo(
                endPoint + controller.transform.up * (height / 2f - radius),
                endPoint - controller.transform.up * (height / 2f - radius),
                radius
            );

            if (IsGrounded)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal);

                if (IsOnSteepSlope)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(hitInfo.point, hitInfo.point + GetSlopeSlideDirection());
                }
            }
        }

        void DrawCapsuleGizmo(Vector3 point1, Vector3 point2, float radius)
        {
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawWireSphere(point1, radius);
            Gizmos.DrawWireSphere(point2, radius);

            var forward = controller.transform.forward * radius;
            var right = controller.transform.right * radius;

            Gizmos.DrawLine(point1 + forward, point2 + forward);
            Gizmos.DrawLine(point1 - forward, point2 - forward);
            Gizmos.DrawLine(point1 + right, point2 + right);
            Gizmos.DrawLine(point1 - right, point2 - right);
        }
    }
}