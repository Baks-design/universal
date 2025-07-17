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
        Coroutine activeCoroutine;

        public bool IsAiming { get; private set; }

        public CameraAiming(CameraData data, CinemachineCamera target)
        {
            this.data = data;
            this.target = target;
            originalFOV = target.Lens.FieldOfView;
        }

        public void ToggleZoom(MonoBehaviour mono)
        {
            IsAiming = !IsAiming;
            StartCoroutine(mono, ZoomRoutine());
        }

        public void SetZoom(MonoBehaviour mono, bool zoomState)
        {
            if (IsAiming == zoomState) return;
            IsAiming = zoomState;
            StartCoroutine(mono, ZoomRoutine());
        }

        void StartCoroutine(MonoBehaviour mono, IEnumerator routine)
        {
            if (activeCoroutine != null)
                mono.StopCoroutine(activeCoroutine);
            activeCoroutine = mono.StartCoroutine(routine);
        }

        IEnumerator ZoomRoutine()
        {
            var startTime = Time.time;
            var currentFOV = target.Lens.FieldOfView;
            var targetFOV = IsAiming ? data.zoomFOV : originalFOV;
            var remainingDistance = Abs(currentFOV - targetFOV);

            if (data.zoomTransitionDuration <= Epsilon)
            {
                target.Lens.FieldOfView = targetFOV;
                yield break;
            }

            while (remainingDistance > 0.1f)
            {
                var elapsed = Time.time - startTime;
                var t = 1f - Exp(-data.zoomSharpness * elapsed * (1f / data.zoomTransitionDuration));

                target.Lens.FieldOfView = Lerp(currentFOV, targetFOV, t);
                remainingDistance = Abs(target.Lens.FieldOfView - targetFOV);

                yield return null;
            }

            target.Lens.FieldOfView = targetFOV;
            activeCoroutine = null;
        }
    }
}