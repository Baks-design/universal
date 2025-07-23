using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraAiming
    {
        readonly CinemachineCamera targetCamera;
        readonly MonoBehaviour coroutineRunner;
        readonly CameraSettings settings;
        readonly float originalFOV;
        Coroutine activeZoomCoroutine;
        const float Epsilon = 0.001f;

        public bool IsAiming { get; set; }

        public CameraAiming(
            MonoBehaviour coroutineRunner,
            CameraSettings settings,
            CinemachineCamera targetCamera)
        {
            this.coroutineRunner = coroutineRunner;
            this.settings = settings;
            this.targetCamera = targetCamera;

            originalFOV = targetCamera.Lens.FieldOfView;
        }

        public void ToggleZoom() => SetZoom(!IsAiming);

        public void SetZoom(bool shouldAim)
        {
            if (IsAiming == shouldAim) return;

            IsAiming = shouldAim;

            StartZoomCoroutine();
        }

        void StartZoomCoroutine()
        {
            if (activeZoomCoroutine != null)
                coroutineRunner.StopCoroutine(activeZoomCoroutine);

            activeZoomCoroutine = coroutineRunner.StartCoroutine(ZoomRoutine());
        }

        IEnumerator ZoomRoutine()
        {
            var startFOV = targetCamera.Lens.FieldOfView;
            var targetFOV = IsAiming ? settings.zoomFOV : originalFOV;

            // Handle instant transition
            if (settings.zoomTransitionDuration <= Epsilon)
            {
                SetFOVImmediate(targetFOV);
                yield break;
            }

            var elapsedTime = 0f;
            var inverseDuration = 1f / settings.zoomTransitionDuration;

            while (elapsedTime < settings.zoomTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = CalculateZoomProgress(elapsedTime * inverseDuration);

                targetCamera.Lens.FieldOfView = Eerp(startFOV, targetFOV, t);
                yield return null;
            }

            SetFOVImmediate(targetFOV);
            activeZoomCoroutine = null;
        }

        float CalculateZoomProgress(float normalizedTime) => 1f - Exp(-settings.zoomSharpness * normalizedTime);

        void SetFOVImmediate(float fov) => targetCamera.Lens.FieldOfView = fov;
    }
}