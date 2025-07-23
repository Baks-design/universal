using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class GroundChecker
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        RaycastHit hitInfo;

        public bool IsGrounded { get; set; }
        public bool JustLanded { get; private set; }
        public RaycastHit HitInfo => hitInfo;

        public GroundChecker(CharacterController controller, PhysicsSettings settings)
        {
            this.controller = controller;
            this.settings = settings;
        }

        public void CheckGround()
        {
            var wasGrounded = IsGrounded;

            IsGrounded = Physics.SphereCast(
                controller.transform.localPosition + controller.center, settings.raySphereRadius, Vector3.down,
                out hitInfo, settings.rayLength + controller.center.y, settings.groundLayer);

            JustLanded = !wasGrounded && IsGrounded;
        }

        public void DrawGroundCheckGizmo()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            var origin = controller.transform.localPosition + controller.center;
            Gizmos.DrawWireSphere(origin, settings.raySphereRadius);
            Gizmos.DrawLine(origin, origin + Vector3.down * (settings.rayLength + controller.center.y));
            Gizmos.DrawWireSphere(origin + Vector3.down * (settings.rayLength + controller.center.y), settings.raySphereRadius);
        }
    }
}
//TODO: Add Sliding Movement
//TODO: Change Sphere by Capsule