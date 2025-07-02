using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        Quaternion targetRotation;

        public bool IsRotating { get; set; }
        public Quaternion TargetRotation => targetRotation;

        public CharacterRotation(
            CharacterMovementController controller,
            CharacterData data)
        {
            this.controller = controller;
            this.data = data;

            targetRotation = controller.transform.rotation;
            IsRotating = false;
        }

        public void RotationRight() => Rotate(90f);

        public void RotationLeft() => Rotate(-90f);

        void Rotate(float angle)
        {
            if (IsRotating) return;

            targetRotation *= Quaternion.Euler(0f, angle, 0f);
            IsRotating = true;
        }

        public void RotateToTarget()
        {
            if (!IsRotating) return;

            var rotationStep = data.rotationSpeed * Time.fixedDeltaTime;
            var angleRemaining = Quaternion.Angle(controller.transform.rotation, targetRotation);

            if (rotationStep >= angleRemaining)
            {
                controller.transform.rotation = targetRotation;
                IsRotating = false;
                return;
            }

            controller.transform.rotation = Quaternion.RotateTowards(
                controller.transform.rotation, targetRotation, rotationStep);
        }
    }
}