using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class ObstacleChecker
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        RaycastHit obstacleHit;

        public bool HasObstacle { get; private set; }
        public RaycastHit ObstacleHit { get => obstacleHit; private set => obstacleHit = value; }

        public ObstacleChecker(CharacterController controller, PhysicsSettings settings)
        {
            this.controller = controller;
            this.settings = settings;
        }

        public void CheckObstacle(Vector3 moveDirection)
        {
            var origin = controller.transform.localPosition + controller.center;

            HasObstacle = Physics.SphereCast(
                origin, settings.rayObstacleSphereRadius, moveDirection,
                out obstacleHit, settings.rayObstacleLength, settings.obstacleLayers);
        }

        public void DrawObstacleCheckGizmo(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero) return;

            Gizmos.color = HasObstacle ? Color.red : Color.yellow;
            var origin = controller.transform.localPosition + controller.center;
            Gizmos.DrawWireSphere(origin, settings.rayObstacleSphereRadius);
            Gizmos.DrawLine(origin, origin + moveDirection * settings.rayObstacleLength);
            Gizmos.DrawWireSphere(origin + moveDirection * settings.rayObstacleLength, settings.rayObstacleSphereRadius);

            if (HasObstacle && obstacleHit.collider != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(obstacleHit.point, 0.1f);
            }
        }
    }
}