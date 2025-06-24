using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Components.Camera
{
    public class CameraAiming
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly float initFOV;
        Coroutine activeCoroutine;
        bool running;

        public bool IsZooming { get; private set; }

        public CameraAiming(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;

            initFOV = target.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (running) return;

            IsZooming = !IsZooming;
            StartCoroutine(mono, ChangeFOVRoutine());
        }

        IEnumerator ChangeFOVRoutine()
        {
            var percent = 0f;
            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = IsZooming ? data.zoomFOV : initFOV;
            var inverseDuration = 1f / data.zoomTransitionDuration;

            while (percent < 1f)
            {
                percent += Time.deltaTime * inverseDuration;
                var smoothPercent = data.zoomCurve.Evaluate(percent);
                target.Lens.FieldOfView = Eerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning, MonoBehaviour mono)
        => StartCoroutine(mono, ChangeRunFOVRoutine(returning));

        private IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : data.runFOV;
            var duration = returning ? data.runReturnTransitionDuration : data.runTransitionDuration;
            var inverseDuration = 1f / duration;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * inverseDuration;
                var smoothPercent = data.runCurve.Evaluate(percent);
                target.Lens.FieldOfView = Eerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }

        void StartCoroutine(MonoBehaviour mono, IEnumerator routine)
        {
            if (activeCoroutine != null)
                mono.StopCoroutine(activeCoroutine);

            activeCoroutine = mono.StartCoroutine(routine);
        }
    }
}