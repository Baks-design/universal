using System.Collections;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRecentering
    {
        readonly CameraData data;
        readonly CameraMovement movement;
        readonly MonoBehaviour monoBehaviour;
        Coroutine recenteringCoroutine;
        Quaternion initialRotation, recenteringStartRotation;
        float rotationTimer, recenteringAngularDistance;
        const float MIN_RECENTER_DURATION = 0.15f;
        
        public bool IsRecentering { get; private set; }

        public CameraRecentering(
            CameraData data,
            CameraMovement movement,
            Quaternion initialRotation,
            MonoBehaviour monoBehaviour)
        {
            this.data = data;
            this.movement = movement;
            this.initialRotation = initialRotation;
            this.monoBehaviour = monoBehaviour;
        }

        public void ReturnToInitialRotation()
        {
            if (IsRecentering) return;

            // Stop any existing coroutine first
            if (recenteringCoroutine != null)
                monoBehaviour.StopCoroutine(recenteringCoroutine);

            recenteringCoroutine = monoBehaviour.StartCoroutine(RecenteringCoroutine());
        }

        IEnumerator RecenteringCoroutine()
        {
            IsRecentering = true;
            rotationTimer = 0f;
            recenteringStartRotation = movement.CurrentRotation;
            recenteringAngularDistance = Quaternion.Angle(
                movement.CurrentRotation, initialRotation
            );

            var dynamicDuration = Max(
                MIN_RECENTER_DURATION,
                data.recenterDuration * (recenteringAngularDistance / 180f)
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

                movement.SetCurrentRotation(newRotation);
                yield return null;
            }

            CompleteRecentering();
            recenteringCoroutine = null;
        }

        void CompleteRecentering()
        {
            movement.SetCurrentRotation(initialRotation);
            movement.ResetTargetToInitialValues(initialRotation);
            IsRecentering = false;
        }
    }
}