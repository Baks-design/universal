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
        readonly float originalFOV;
        Coroutine activeCoroutine = null;

        public bool IsZooming { get; private set; } = false;

        public CameraAiming(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;
            
            originalFOV = target.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            IsZooming = !IsZooming;
            StartCoroutine(mono, ChangeFOVRoutine());
        }

        IEnumerator ChangeFOVRoutine()
        {
            var percent = 0f;
            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = IsZooming ? data.zoomFOV : originalFOV;

            if (data.zoomTransitionDuration <= 0f)
            {
                target.Lens.FieldOfView = targetFOV;
                yield break;
            }

            var inverseDuration = 1f / data.zoomTransitionDuration;

            while (percent < 1f)
            {
                percent += Time.deltaTime * inverseDuration;
                var smoothPercent = data.zoomCurve?.Evaluate(percent) ?? percent;
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