using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        Coroutine recenteringCoroutine;
        Quaternion currentRotation;
        Quaternion initialRotation;
        Quaternion recenteringStartRotation;
        const float MinRecenterDuration = 0.15f;
        const float AngularDistanceThreshold = 0.1f;
        const float FullRotationDegrees = 180f;
        float desiredTargetYaw;
        float desiredTargetPitch;
        float targetYaw;
        float targetPitch;
        float yawSmoothVelocity;
        float pitchSmoothVelocity;
        float rotationTimer;
        float recenteringAngularDistance;

        public bool IsRecentering { get; private set; }

        public CameraRotation(
            CameraData data,
            CinemachineCamera target)
        {
            this.data = data;
            this.target = target;

            IsRecentering = false;
            initialRotation = target.transform.localRotation;
            currentRotation = initialRotation;
            ResetTargetToInitialValues();
        }

        public void ProcessRotation(Vector2 lookInput)
        {
            if (IsRecentering) return;

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * Time.deltaTime;
            desiredTargetYaw = Helpers.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);

            desiredTargetPitch -= lookInput.y * data.sensitivityAmount.y * Time.deltaTime;
            desiredTargetPitch = Helpers.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            var yawSmoothTime = Approximately(data.smoothAmount.x, 0f) ? float.MaxValue : 1f / data.smoothAmount.x;
            var pitchSmoothTime = Approximately(data.smoothAmount.y, 0f) ? float.MaxValue : 1f / data.smoothAmount.y;

            targetYaw = SmoothDamp(targetYaw, desiredTargetYaw, ref yawSmoothVelocity, yawSmoothTime, Infinity, Time.deltaTime);
            targetPitch = SmoothDamp(targetPitch, desiredTargetPitch, ref pitchSmoothVelocity, pitchSmoothTime, Infinity, Time.deltaTime);

            currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            target.transform.localRotation = currentRotation;
        }

        public void ReturnToInitialRotation(MonoBehaviour mono)
        {
            if (IsRecentering || currentRotation == initialRotation) return;
            recenteringCoroutine ??= mono.StartCoroutine(RecenteringCoroutine());
        }

        IEnumerator RecenteringCoroutine()
        {
            try
            {
                IsRecentering = true;
                rotationTimer = 0f;
                recenteringStartRotation = currentRotation;

                recenteringAngularDistance = Quaternion.Angle(currentRotation, initialRotation);
                if (recenteringAngularDistance < AngularDistanceThreshold)
                {
                    CompleteRecentering();
                    yield break;
                }

                var dynamicDuration = Max(
                    MinRecenterDuration,
                    data.recenterDuration * (recenteringAngularDistance / FullRotationDegrees)
                );
                while (rotationTimer < dynamicDuration)
                {
                    rotationTimer += Time.deltaTime;
                    var progress = Clamp01(rotationTimer / dynamicDuration);
                    var easedProgress = Smooth01(progress);

                    var newRotation = Quaternion.Slerp(
                        recenteringStartRotation,
                        initialRotation,
                        easedProgress
                    );

                    SetCurrentRotation(newRotation);
                    yield return null;
                }
            }
            finally
            {
                CompleteRecentering();
                recenteringCoroutine = null;
            }
        }

        void CompleteRecentering()
        {
            try
            {
                SetCurrentRotation(initialRotation);
                ResetTargetToInitialValues();
            }
            finally
            {
                IsRecentering = false;
            }
        }

        void SetCurrentRotation(Quaternion rotation)
        {
            currentRotation = rotation;
            target.transform.localRotation = rotation;
        }

        void ResetTargetToInitialValues()
        {
            desiredTargetYaw = Helpers.NormalizeAngle(initialRotation.eulerAngles.y);
            desiredTargetPitch = Helpers.NormalizeAngle(initialRotation.eulerAngles.x);
            targetYaw = desiredTargetYaw;
            targetPitch = desiredTargetPitch;
            yawSmoothVelocity = 0f;
            pitchSmoothVelocity = 0f;
        }
    }
}