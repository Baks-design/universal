using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly CharacterPlayerController controller;
        const float MINRECENTERDURATION = 0.15f;
        const float ANGULARDISTANCETHRESHOLD = 0.1f;
        const float FULLROTATIONDEGREES = 180f;
        const float INPUTDEADZONE = 0.01f;
        const float MAXDELTATIME = 0.0333f;
        const float TIMEOUT = 2f;
        Coroutine recenteringCoroutine;
        Quaternion currentRotation;
        Quaternion initialRotation;
        Quaternion recenteringStartRotation;
        float desiredTargetYaw;
        float desiredTargetPitch;
        float targetYaw;
        float targetPitch;
        float yawSmoothVelocity;
        float pitchSmoothVelocity;
        float rotationTimer;
        float recenteringAngularDistance;
        float lastInputMagnitude;

        public bool IsRecentering { get; private set; }

        public CameraRotation(CameraData data, CinemachineCamera target, CharacterPlayerController controller)
        {
            this.data = data;
            this.target = target;
            this.controller= controller;

            initialRotation = target.transform.localRotation;
            ResetToInitialValues();
        }

        public void ProcessRotation(Vector2 lookInput)
        {
            if (IsRecentering) return;

            var currentInputMag = lookInput.sqrMagnitude;
            if (currentInputMag < INPUTDEADZONE && lastInputMagnitude < INPUTDEADZONE)
            {
                lastInputMagnitude = currentInputMag;
                return;
            }
            lastInputMagnitude = currentInputMag;

            var deltaTime = Min(Time.deltaTime, MAXDELTATIME);

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * deltaTime;
            desiredTargetPitch -= lookInput.y * data.sensitivityAmount.y * deltaTime;

            desiredTargetYaw = Helpers.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = Helpers.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            var yawSmoothTime = Approximately(data.smoothAmount.x, 0f) ? 0f : 1f / data.smoothAmount.x;
            var pitchSmoothTime = Approximately(data.smoothAmount.y, 0f) ? 0f : 1f / data.smoothAmount.y;

            targetYaw = SmoothDamp(targetYaw, desiredTargetYaw, ref yawSmoothVelocity, yawSmoothTime);
            targetPitch = SmoothDamp(targetPitch, desiredTargetPitch, ref pitchSmoothVelocity, pitchSmoothTime);

            currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation,
                currentRotation,
                1f - Exp(-data.smoothAmount.magnitude * deltaTime)
            );
        }

        public void ReturnToInitialRotation(MonoBehaviour mono)
        {
            if (!controller.IsCurrentStateEqual(controller.MovementState) ||
                IsRecentering ||
                Quaternion.Angle(currentRotation, initialRotation) < ANGULARDISTANCETHRESHOLD)
                return;
            
            if (recenteringCoroutine != null)
                mono.StopCoroutine(recenteringCoroutine);
            recenteringCoroutine = mono.StartCoroutine(RecenteringCoroutine());
        }

        IEnumerator RecenteringCoroutine()
        {
            var startTime = Time.time;

            IsRecentering = true;
            rotationTimer = 0f;
            recenteringStartRotation = currentRotation;
            recenteringAngularDistance = Quaternion.Angle(currentRotation, initialRotation);

            var dynamicDuration = Max(
                MINRECENTERDURATION,
                data.recenterDuration * (recenteringAngularDistance / FULLROTATIONDEGREES)
            );

            while (rotationTimer < dynamicDuration && (Time.time - startTime) < TIMEOUT)
            {
                if (!IsRecentering) yield break;

                rotationTimer += Time.deltaTime;
                var progress = Clamp01(rotationTimer / dynamicDuration);
                var easedProgress = Smoother01(progress);

                currentRotation = Quaternion.Slerp(
                    recenteringStartRotation,
                    initialRotation,
                    easedProgress
                );

                target.transform.localRotation = currentRotation;
                yield return null;
            }

            CompleteRecentering();
            recenteringCoroutine = null;
        }

        void CompleteRecentering()
        {
            currentRotation = initialRotation;
            target.transform.localRotation = initialRotation;
            ResetToInitialValues();
            IsRecentering = false;
        }

        void ResetToInitialValues()
        {
            var euler = initialRotation.eulerAngles;
            desiredTargetYaw = Helpers.NormalizeAngle(euler.y);
            desiredTargetPitch = Helpers.NormalizeAngle(euler.x);
            targetYaw = desiredTargetYaw;
            targetPitch = desiredTargetPitch;
            yawSmoothVelocity = 0f;
            pitchSmoothVelocity = 0f;
        }
    }
}