using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly CharacterData data;
        Quaternion targetRotation;

        public bool IsRotating { get; private set; }

        public CharacterRotation(
            CharacterMovementController controller,
            CharacterData data)
        {
            this.controller = controller;
            this.data = data;

            targetRotation = controller.transform.localRotation;
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

            var locaRotation = controller.transform.localRotation;
            var angleRemaining = Quaternion.Angle(locaRotation, targetRotation);
            var rotationStep = data.rotationSpeed * Time.deltaTime;

            if (rotationStep >= angleRemaining)
            {
                controller.transform.localRotation = targetRotation;
                IsRotating = false;
                return;
            }

            controller.transform.localRotation = Quaternion.RotateTowards(
                locaRotation, targetRotation, rotationStep);
        }
    }
}