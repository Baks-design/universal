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
        readonly Transform transform;
        readonly CharacterPlayerController playerController;
        readonly CharacterCameraController cameraController;
        const float MINRECENTERDURATION = 0.3f;
        const float ANGULARDISTANCETHRESHOLD = 0.1f;
        Coroutine recenteringCoroutine;
        Quaternion currentRotation;
        Quaternion initialRotation;
        float desiredTargetYaw;
        float desiredTargetPitch;
        float targetYaw;
        float targetPitch;
        float yawSmoothVelocity;
        float pitchSmoothVelocity;
        float rotationTimer;

        public bool IsRecentering { get; private set; }

        public CameraRotation(
            CameraData data,
            CinemachineCamera camera,
            CharacterPlayerController playerController,
            CharacterCameraController cameraController)
        {
            this.data = data;
            transform = camera.transform;
            this.playerController = playerController;
            this.cameraController = cameraController;

            initialRotation = camera.transform.localRotation;
            ResetToInitialValues();
        }

        public void ProcessRotation(Vector2 lookInput)
        {
            if (!playerController.IsCurrentStateEqual(playerController.InvestigationState) &&
                (!playerController.IsCurrentStateEqual(playerController.CombatState) ||
                IsRecentering))
                return;

            var frameScale = Time.deltaTime * 60f;
            var sensitivityScale = cameraController.CameraAiming.IsAiming ? data.aimingSensitivityMultiplier : 1f;

            desiredTargetYaw += lookInput.x * data.sensitivityAmount.x * frameScale * sensitivityScale;
            desiredTargetPitch -= lookInput.y * data.sensitivityAmount.y * frameScale * sensitivityScale;

            desiredTargetYaw = Helpers.ClampAngle(desiredTargetYaw, data.horizontalClamp.x, data.horizontalClamp.y);
            desiredTargetPitch = Helpers.ClampAngle(desiredTargetPitch, data.verticalClamp.x, data.verticalClamp.y);

            targetYaw = SmoothDamp(
                targetYaw, desiredTargetYaw, ref yawSmoothVelocity,
                data.smoothAmount.x, data.maxRotationSpeed, Time.deltaTime
            );
            targetPitch = SmoothDamp(
                targetPitch, desiredTargetPitch, ref pitchSmoothVelocity,
                data.smoothAmount.y, data.maxRotationSpeed, Time.deltaTime
            );

            currentRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            transform.localRotation = currentRotation;
        }

        public void ReturnToInitialRotation(MonoBehaviour mono)
        {
            if (IsRecentering || !playerController.IsCurrentStateEqual(playerController.MovementState))
                return;

            var currentAngle = Quaternion.Angle(currentRotation, initialRotation);
            if (currentAngle < ANGULARDISTANCETHRESHOLD)
                return;

            if (recenteringCoroutine != null)
                mono.StopCoroutine(recenteringCoroutine);
            recenteringCoroutine = mono.StartCoroutine(RecenteringCoroutine());
        }

        IEnumerator RecenteringCoroutine()
        {
            IsRecentering = true;
            rotationTimer = 0f;

            var startRot = currentRotation;
            var angularDistance = Quaternion.Angle(startRot, initialRotation);

            var duration = Max(
                MINRECENTERDURATION,
                angularDistance * (1f / (data.recenterSharpness * 100f))
            );

            while (rotationTimer < duration)
            {
                if (!playerController.IsCurrentStateEqual(playerController.MovementState))
                {
                    IsRecentering = false;
                    yield break;
                }

                rotationTimer += Time.deltaTime;
                var t = Clamp01(rotationTimer * (1f / duration));

                currentRotation = Quaternion.Slerp(startRot, initialRotation, t * (2f - t));

                transform.localRotation = currentRotation;
                UpdateRotationVariables(currentRotation.eulerAngles);

                yield return null;
            }

            CompleteRecentering();
        }

        void UpdateRotationVariables(Vector3 eulerAngles)
        {
            desiredTargetYaw = Helpers.NormalizeAngle(eulerAngles.y);
            desiredTargetPitch = Helpers.NormalizeAngle(eulerAngles.x);
            targetYaw = desiredTargetYaw;
            targetPitch = desiredTargetPitch;
        }

        void CompleteRecentering()
        {
            currentRotation = initialRotation;
            transform.localRotation = initialRotation;
            UpdateRotationVariables(initialRotation.eulerAngles);
            ResetToInitialValues();
        }

        void ResetToInitialValues()
        {
            yawSmoothVelocity = 0f;
            pitchSmoothVelocity = 0f;
            IsRecentering = false;
            recenteringCoroutine = null;
        }
    }
}