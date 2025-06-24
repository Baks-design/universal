using UnityEngine;
using Unity.Cinemachine;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraSwaying
    {
        readonly CameraData data;
        readonly CinemachineCamera camera;
        Vector3 currentEulerAngles;
        bool differentDirection;
        float scrollSpeed, currentXInput, previousXInput;
        const float MaxScrollSpeed = 1f;

        public CameraSwaying(CameraData data, CinemachineCamera camera)
        {
            this.data = data;
            this.camera = camera;

            currentEulerAngles = camera.transform.localEulerAngles;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            currentXInput = rawXInput;

            if (rawXInput != 0f)
                HandleMovementInput(inputVector.x);
            else
                HandleNoInput();

            scrollSpeed = Clamp(scrollSpeed, -MaxScrollSpeed, MaxScrollSpeed);
            ApplySway();
            previousXInput = currentXInput;
        }

        void HandleMovementInput(float xAmount)
        {
            if (currentXInput != previousXInput && previousXInput != 0f)
                differentDirection = true;

            var speedMultiplier = differentDirection ? data.changeDirectionMultiplier : 1f;
            scrollSpeed += xAmount * data.swaySpeed * Time.deltaTime * speedMultiplier;
        }

        void HandleNoInput()
        {
            if (currentXInput == previousXInput)
                differentDirection = false;

            scrollSpeed = Lerp(scrollSpeed, 0f, Time.deltaTime * data.returnSpeed);
        }

        void ApplySway()
        {
            if (data.swayCurve == null) return;

            currentEulerAngles.z = CalculateSwayAmount();
            camera.transform.localEulerAngles = currentEulerAngles;
        }

        float CalculateSwayAmount()
        {
            var evaluatedAmount = data.swayCurve.Evaluate(Abs(scrollSpeed) * data.swayAmount);
            return scrollSpeed < 0f ? -evaluatedAmount : evaluatedAmount;
        }
    }
}