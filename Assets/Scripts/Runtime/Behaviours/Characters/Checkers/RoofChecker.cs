using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class RoofChecker
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        readonly float initHeight;

        public bool HasRoof { get; private set; }

        public RoofChecker(CharacterController controller, PhysicsSettings settings)
        {
            this.controller = controller;
            this.settings = settings;

            initHeight = controller.height;
        }

        public void CheckIfRoof()
        {
            HasRoof = Physics.SphereCast(
                controller.transform.localPosition, settings.raySphereRadius, Vector3.up,
                out var _, initHeight);
        }

        public void DrawRoofCheckGizmo()
        {
            Gizmos.color = HasRoof ? Color.red : Color.green;
            var position = controller.transform.localPosition;
            Gizmos.DrawWireSphere(position, settings.raySphereRadius);
            Gizmos.DrawLine(position, position + Vector3.up * initHeight);
            Gizmos.DrawWireSphere(position + Vector3.up * initHeight, settings.raySphereRadius);
        }
    }
}