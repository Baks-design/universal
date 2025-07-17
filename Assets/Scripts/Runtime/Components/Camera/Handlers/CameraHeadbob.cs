using UnityEngine;
using Universal.Runtime.Components.Camera;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CameraHeadbob
    {
        readonly Transform transform;
        readonly CameraData data;
        Vector3 originalLocalPos;
        Vector3 currentVelocity;
        float headbobCycle;

        public CameraHeadbob(Transform transform, CameraData data)
        {
            this.transform = transform;
            this.data = data;
            originalLocalPos = transform.localPosition;
        }

        public void ProcessHeadbob(IGridMover mover)
        {
            if (mover.IsMoving)
            {
                var intensity = mover.IsRunning ? data.runBobIntensity : data.walkBobIntensity;
                var frequency = mover.IsRunning ? data.runBobFrequency : data.walkBobFrequency;

                headbobCycle += Time.deltaTime * frequency;

                var targetOffset = new Vector3(0f, Sin(headbobCycle * PI * data.sinMultiplier) * intensity, 0f);

                transform.localPosition = Vector3.SmoothDamp(
                    transform.localPosition, originalLocalPos + targetOffset, ref currentVelocity, data.bobSmoothTime);
            }
            else
                transform.localPosition = Vector3.SmoothDamp(
                    transform.localPosition, originalLocalPos, ref currentVelocity, data.bobReturnSmoothTime
                );
        }
    }
}