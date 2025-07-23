using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class SwayingHandler
    {
        readonly CameraSettings settings;
        readonly Transform transform;
        float scrollSpeed;
        float currentInput;
        float previousInput;
        bool isChangingDirection;

        public SwayingHandler(CameraSettings settings, Transform transform)
        {
            this.settings = settings;
            this.transform = transform;
        }

        public void ApplySway(Vector3 inputVector, float rawXInput)
        {
            currentInput = rawXInput;

            if (Abs(rawXInput) > Epsilon)
            {
                CheckDirectionChange();
                UpdateScrollSpeed(inputVector.x);
            }
            else
                ResetSway();

            ClampScrollSpeed();
            ApplySwayToCamera();

            previousInput = currentInput;
        }

        void CheckDirectionChange()
        {
            if (!Approximately(currentInput, previousInput) && !Approximately(previousInput, 0f))
                isChangingDirection = true;
        }

        void UpdateScrollSpeed(float xInput)
        {
            var speedMultiplier = isChangingDirection ? settings.changeDirectionMultiplier : 1f;
            scrollSpeed += xInput * settings.swaySpeed * Time.deltaTime * speedMultiplier;
        }

        void ResetSway()
        {
            if (Approximately(currentInput, previousInput))
                isChangingDirection = false;

            scrollSpeed = Helpers.ExpDecay(scrollSpeed, 0f, Time.deltaTime * settings.returnSpeed);
        }

        void ClampScrollSpeed() => scrollSpeed = Clamp(scrollSpeed, -1f, 1f);

        void ApplySwayToCamera()
        {
            var swayAmount = settings.swayCurve.Evaluate(Abs(scrollSpeed)) * settings.swayAmount;
            var swayFinalAmount = Sign(scrollSpeed) * swayAmount;
            var newRotation = transform.localEulerAngles;
            newRotation.z = swayFinalAmount;
            transform.localEulerAngles = newRotation;
        }
    }
}
