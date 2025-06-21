using Unity.Cinemachine;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public class CameraSwaying
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        bool diffrentDirection;
        float scrollSpeed, xAmountThisFrame, xAmountPreviousFrame;

        public CameraSwaying(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            var xAmount = inputVector.x;

            xAmountThisFrame = rawXInput;

            if (rawXInput != 0f) // if we have some input
            {
                // if our previous dir is not equal to current one and the previous one was not idle
                if (xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0)
                    diffrentDirection = true;

                // then we multiplier our scroll so when changing direction it will sway to the other direction faster
                var speedMultiplier = diffrentDirection ? data.changeDirectionMultiplier : 1f;
                scrollSpeed += xAmount * data.swaySpeed * Time.deltaTime * speedMultiplier;
            }
            else // if we are not moving so there is no input
            {
                if (xAmountThisFrame == xAmountPreviousFrame) // check if our previous dir equals current dir
                    diffrentDirection = false; // if yes we want to reset this bool so basically it can be used correctly once we move again

                scrollSpeed = Mathf.Lerp(scrollSpeed, 0f, Time.deltaTime * data.returnSpeed);
            }

            scrollSpeed = Mathf.Clamp(scrollSpeed, -1f, 1f);

            float swayFinalAmount;
            if (scrollSpeed < 0f)
                swayFinalAmount = -data.swayCurve.Evaluate(scrollSpeed * -data.swayAmount);
            else
                swayFinalAmount = data.swayCurve.Evaluate(scrollSpeed * -data.swayAmount);

            Vector3 swayVector;
            swayVector.z = swayFinalAmount;

            target.transform.localEulerAngles = new Vector3(
                target.transform.localEulerAngles.x,
                target.transform.localEulerAngles.y,
                swayVector.z
            );

            xAmountPreviousFrame = xAmountThisFrame;
        }
    }
}