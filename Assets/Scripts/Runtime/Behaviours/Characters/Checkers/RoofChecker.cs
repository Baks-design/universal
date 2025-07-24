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
            var radius = controller.radius; 
            var height = controller.height; 

            var center = controller.transform.localPosition + controller.center;
            var point1 = center + controller.transform.up * (height / 2f - radius);
            var point2 = center - controller.transform.up * (height / 2f - radius);

            HasRoof = Physics.CapsuleCast(
                point1,
                point2,
                radius,
                controller.transform.up,
                out _,
                initHeight,
                settings.roofLayers); 
        }

        public void DrawRoofCheckGizmo()
        {
            Gizmos.color = HasRoof ? Color.red : Color.green;

            var radius = controller.radius;
            var height = controller.height;
            var center = controller.transform.localPosition + controller.center;

            var point1 = center + controller.transform.up * (height / 2f - radius);
            var point2 = center - controller.transform.up  * (height / 2f - radius);

            DrawCapsuleGizmo(point1, point2, radius);

            Gizmos.DrawLine(center, center + controller.transform.up * initHeight);

            var endPoint = center + controller.transform.up * initHeight;
            DrawCapsuleGizmo(
                endPoint + controller.transform.up * (height / 2f - radius),
                endPoint - controller.transform.up * (height / 2f - radius),
                radius
            );
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