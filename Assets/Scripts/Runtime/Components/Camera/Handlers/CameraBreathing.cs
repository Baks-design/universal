using Unity.Cinemachine;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public class CameraBreathing
    {
        readonly PerlinNoiseData perlinNoiseData;
        readonly CameraData cameraData;
        readonly CinemachineCamera target;
        readonly PerlinNoiseScroller perlinNoiseScroller;
        Vector3 finalRot, finalPos;

        public CameraBreathing(
            PerlinNoiseData perlinNoiseData, CameraData cameraData, CinemachineCamera target)
        {
            this.perlinNoiseData = perlinNoiseData;
            this.cameraData = cameraData;
            this.target = target;

            perlinNoiseScroller = new PerlinNoiseScroller(perlinNoiseData);
        }

        public void UpdateBreathing()
        {
            perlinNoiseScroller.UpdateNoise();

            var posOffset = Vector3.zero;
            var rotOffset = Vector3.zero;

            switch (perlinNoiseData.transformTarget)
            {
                case TransformTarget.Position:
                    if (cameraData.x)
                        posOffset.x += perlinNoiseScroller.Noise.x;

                    if (cameraData.y)
                        posOffset.y += perlinNoiseScroller.Noise.y;

                    if (cameraData.z)
                        posOffset.z += perlinNoiseScroller.Noise.z;

                    finalPos.x = cameraData.x ? posOffset.x : target.transform.localPosition.x;
                    finalPos.y = cameraData.y ? posOffset.y : target.transform.localPosition.y;
                    finalPos.z = cameraData.z ? posOffset.z : target.transform.localPosition.z;

                    target.transform.localPosition = finalPos;
                    break;
                case TransformTarget.Rotation:
                    if (cameraData.x)
                        rotOffset.x += perlinNoiseScroller.Noise.x;

                    if (cameraData.y)
                        rotOffset.y += perlinNoiseScroller.Noise.y;

                    if (cameraData.z)
                        rotOffset.z += perlinNoiseScroller.Noise.z;

                    finalRot.x = cameraData.x ? rotOffset.x : target.transform.localEulerAngles.x;
                    finalRot.y = cameraData.y ? rotOffset.y : target.transform.localEulerAngles.y;
                    finalRot.z = cameraData.z ? rotOffset.z : target.transform.localEulerAngles.z;

                    target.transform.localEulerAngles = finalRot;

                    break;
                case TransformTarget.Both:
                    if (cameraData.x)
                    {
                        posOffset.x += perlinNoiseScroller.Noise.x;
                        rotOffset.x += perlinNoiseScroller.Noise.x;
                    }

                    if (cameraData.y)
                    {
                        posOffset.y += perlinNoiseScroller.Noise.y;
                        rotOffset.y += perlinNoiseScroller.Noise.y;
                    }

                    if (cameraData.z)
                    {
                        posOffset.z += perlinNoiseScroller.Noise.z;
                        rotOffset.z += perlinNoiseScroller.Noise.z;
                    }

                    finalPos.x = cameraData.x ? posOffset.x : target.transform.localPosition.x;
                    finalPos.y = cameraData.y ? posOffset.y : target.transform.localPosition.y;
                    finalPos.z = cameraData.z ? posOffset.z : target.transform.localPosition.z;

                    finalRot.x = cameraData.x ? rotOffset.x : target.transform.localEulerAngles.x;
                    finalRot.y = cameraData.y ? rotOffset.y : target.transform.localEulerAngles.y;
                    finalRot.z = cameraData.z ? rotOffset.z : target.transform.localEulerAngles.z;

                    target.transform.localPosition = finalPos;
                    target.transform.localEulerAngles = finalRot;
                    break;
            }
        }
    }
}