using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    public class CameraAiming
    {
        readonly CameraData data;
        readonly CinemachineCamera target;
        readonly float initFOV;
        bool running;
        IEnumerator changeFOVRoutine, changeRunFOVRoutine;

        public bool IsZooming { get; set; }

        public CameraAiming(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;

            initFOV = target.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (running)
            {
                IsZooming = !IsZooming;
                return;
            }

            if (changeRunFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);

            if (changeFOVRoutine != null)
                mono.StopCoroutine(changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine();
            mono.StartCoroutine(changeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            var percent = 0f;
            var smoothPercent = 0f;

            var speed = 1f / data.zoomTransitionDuration;

            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = IsZooming ? initFOV : data.zoomFOV;

            IsZooming = !IsZooming;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                smoothPercent = data.zoomCurve.Evaluate(percent);
                target.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning, MonoBehaviour mono)
        {
            if (changeFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);

            if (changeRunFOVRoutine != null)
                mono.StopCoroutine(changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var smoothPercent = 0f;

            var duration = returning ? data.runReturnTransitionDuration : data.runTransitionDuration;
            var speed = 1f / duration;

            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : data.runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                smoothPercent = data.runCurve.Evaluate(percent);
                target.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}