using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterHeadBob
    {
        readonly CharacterMovementController controller;
        readonly Transform headTransform;
        readonly CharacterData data;
        readonly float originalBaseHeight;
        float timer;
        float currentBobAmount;
        bool isBobbing;

        public float BaseHeight { get; private set; }
        public float CurrentYPos { get; private set; }

        public CharacterHeadBob(
            CharacterMovementController controller,
            CharacterData data,
            Transform headTransform)
        {
            this.controller = controller;
            this.data = data;
            this.headTransform = headTransform;

            originalBaseHeight = headTransform.localPosition.y;
            BaseHeight = originalBaseHeight;
        }

        public void Update()
        {
            HandleMovementBob();
            HandleLandingEffect();
            ApplyHeadPosition();
        }

        void HandleMovementBob()
        {
            if (controller.CharacterMovement.IsMoving &&
                !controller.CharacterRotation.IsRotating)
            {
                isBobbing = true;

                timer += Time.deltaTime * data.walkBobSpeed;

                var targetAmount = data.walkBobAmount;

                currentBobAmount = Lerp(
                    currentBobAmount,
                    targetAmount,
                    data.transitionSpeed * Time.deltaTime
                );
            }
            else if (isBobbing)
            {
                isBobbing = false;

                currentBobAmount = Lerp(
                    currentBobAmount,
                    0f,
                    data.transitionSpeed * Time.deltaTime
                );

                if (currentBobAmount < 0.01f)
                    currentBobAmount = 0f;
            }
        }

        void HandleLandingEffect()
        {
            if (!controller.CharacterCollision.JustLanded) return;

            timer = PI * 0.5f;
            currentBobAmount = data.landBobAmount;
        }

        public void AdjustBaseHeight(float newHeight)
        {
            BaseHeight = newHeight;
            ApplyHeadPosition();
        }

        public void ResetBaseHeight()
        {
            BaseHeight = originalBaseHeight;
            ApplyHeadPosition();
        }

        void ApplyHeadPosition()
        {
            var yPos = BaseHeight;

            if (currentBobAmount > 0f)
                yPos += Sin(timer) * currentBobAmount;

            headTransform.localPosition = new Vector3(
                headTransform.localPosition.x,
                yPos,
                headTransform.localPosition.z
            );

            CurrentYPos = yPos;
        }

    }
}