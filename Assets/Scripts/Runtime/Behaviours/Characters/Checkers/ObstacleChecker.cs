using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class ObstacleChecker
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        readonly CharacterMovementController movement;
        RaycastHit obstacleHit;

        public bool HasObstacle { get; private set; }
        public RaycastHit ObstacleHit { get => obstacleHit; private set => obstacleHit = value; }

        public ObstacleChecker(
            CharacterController controller,
            PhysicsSettings settings,
            CharacterMovementController movement)
        {
            this.controller = controller;
            this.settings = settings;
            this.movement = movement;
        }

        public void CheckObstacle()
        {
            HasObstacle = Physics.SphereCast(
                controller.transform.localPosition + controller.center,
                settings.rayObstacleSphereRadius,
                movement.Direction,
                out obstacleHit,
                settings.rayObstacleLength,
                settings.obstacleLayers);
        }

        public void DrawObstacleCheckGizmo()
        {
            var dir = movement.Direction;
            if (dir == Vector3.zero) return;

            Gizmos.color = HasObstacle ? Color.red : Color.yellow;
            var origin = controller.transform.localPosition + controller.center;
            Gizmos.DrawWireSphere(origin, settings.rayObstacleSphereRadius);
            Gizmos.DrawLine(origin, origin + dir * settings.rayObstacleLength);
            Gizmos.DrawWireSphere(origin + dir * settings.rayObstacleLength, settings.rayObstacleSphereRadius);

            if (HasObstacle && obstacleHit.collider != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(obstacleHit.point, 0.1f);
            }
        }
    }
}