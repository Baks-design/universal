using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class AimingHandler
    {
        readonly CinemachineCamera camera;
        readonly CameraSettings settings;
        readonly float initialFOV;
        Coroutine activeFOVCoroutine;
        bool running;

        public bool IsZooming { get; private set; }

        public AimingHandler(CinemachineCamera camera, CameraSettings settings)
        {
            this.camera = camera;
            this.settings = settings;

            initialFOV = camera.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (running)
            {
                IsZooming = !IsZooming;
                return;
            }

            StopActiveCoroutine(mono);
            activeFOVCoroutine = mono.StartCoroutine(ChangeFOVRoutine());
        }

        public void ChangeRunFOV(MonoBehaviour mono, bool returning)
        {
            StopActiveCoroutine(mono);
            activeFOVCoroutine = mono.StartCoroutine(ChangeRunFOVRoutine(returning));
        }

        void StopActiveCoroutine(MonoBehaviour mono)
        {
            if (activeFOVCoroutine == null) return;

            mono.StopCoroutine(activeFOVCoroutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            var isZooming = IsZooming;
            var startFOV = camera.Lens.FieldOfView;
            var targetFOV = isZooming ? initialFOV : settings.zoomFOV;

            IsZooming = !IsZooming;

            yield return LerpFieldOfView(startFOV, targetFOV, settings.zoomTransitionDuration, settings.zoomCurve);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var startFOV = camera.Lens.FieldOfView;
            var targetFOV = returning ? initialFOV : settings.runFOV;
            var duration = returning ? settings.runReturnTransitionDuration : settings.runTransitionDuration;

            running = !returning;

            yield return LerpFieldOfView(startFOV, targetFOV, duration, settings.runCurve);
        }

        IEnumerator LerpFieldOfView(float startFOV, float targetFOV, float duration, AnimationCurve curve)
        {
            if (duration <= 0f)
            {
                camera.Lens.FieldOfView = targetFOV;
                yield break;
            }

            var elapsed = 0f;
            var speed = 1f / duration;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * speed;
                var smoothedProgress = curve.Evaluate(elapsed);
                camera.Lens.FieldOfView = Eerp(startFOV, targetFOV, smoothedProgress);
                yield return null;
            }

            camera.Lens.FieldOfView = targetFOV;
        }
    }
}