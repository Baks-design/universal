using Unity.Cinemachine;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraSwaying
    {
        readonly CameraData data;
        readonly Transform targetTransform;
        bool differentDirection;
        float scrollSpeed, xAmountThisFrame, xAmountPreviousFrame;
        Vector3 currentEulerAngles;

        public CameraSwaying(CameraData data, CinemachineCamera target)
        {
            this.data = data;

            targetTransform = target.transform;
            currentEulerAngles = targetTransform.localEulerAngles;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            xAmountThisFrame = rawXInput;

            if (rawXInput != 0f)
                HandleMovementInput(inputVector.x);
            else
                HandleNoInput();

            scrollSpeed = Clamp(scrollSpeed, -1f, 1f);
            ApplySway();
            xAmountPreviousFrame = xAmountThisFrame;
        }

        void HandleMovementInput(float xAmount)
        {
            // Check if direction changed
            if (xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0f)
                differentDirection = true;

            var speedMultiplier = differentDirection ? data.changeDirectionMultiplier : 1f;
            scrollSpeed += xAmount * data.swaySpeed * Time.deltaTime * speedMultiplier;
        }

        void HandleNoInput()
        {
            if (xAmountThisFrame == xAmountPreviousFrame)
                differentDirection = false;

            scrollSpeed = Lerp(scrollSpeed, 0f, Time.deltaTime * data.returnSpeed);
        }

        void ApplySway()
        {
            var swayFinalAmount = CalculateSwayAmount();
            // Update only the Z axis to minimize property access
            currentEulerAngles.z = swayFinalAmount;
            targetTransform.localEulerAngles = currentEulerAngles;
        }

        float CalculateSwayAmount()
        {
            var evaluatedAmount = data.swayCurve.Evaluate(Abs(scrollSpeed) * data.swayAmount);
            return scrollSpeed < 0f ? -evaluatedAmount : evaluatedAmount;
        }
    }
}