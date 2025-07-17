using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CurveBasedMovementAnimator : IMovementAnimator
    {
        readonly CharacterData data;
        Vector3? startPosition;
        Vector3? targetPosition;
        Vector3 movementDirection;
        Vector3 currentVelocity;
        float progress;
        float effectiveMoveDuration;

        public bool IsAnimating => startPosition.HasValue;
        public Quaternion CurrentRotation => Quaternion.identity;
        public Vector3 CurrentPosition { get; private set; }

        public CurveBasedMovementAnimator(CharacterData data)
        {
            this.data = data;
            CurrentPosition = Vector3.zero;
        }

        public void AnimateMovement(Vector3 start, Vector3 end, float duration)
        {
            if (duration <= Epsilon)
            {
                CurrentPosition = end;
                return;
            }

            startPosition = start;
            targetPosition = end;
            CurrentPosition = start;
            progress = 0f;
            currentVelocity = Vector3.zero;
            movementDirection = (end - start).normalized;
            effectiveMoveDuration = duration * data.moveFactor;
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating) return;

            progress += Time.deltaTime * (1f / effectiveMoveDuration);

            var easedTime = data.movementCurve.Evaluate(progress);

            var targetPos = Vector3.Lerp(startPosition.Value, targetPosition.Value, easedTime);

            if (data.enableOvershoot && progress < 0.9f)
            {
                var overshootFactor = data.overshootCurve.Evaluate(progress);
                targetPos += data.overshootDistance * overshootFactor * movementDirection;
            }

            CurrentPosition = Vector3.SmoothDamp(
                CurrentPosition, targetPos, ref currentVelocity, data.smoothTime, data.maxSpeed);

            if (progress >= 1f)
                CompleteAnimation();
        }

        void CompleteAnimation()
        {
            CurrentPosition = targetPosition.Value;
            startPosition = null;
            targetPosition = null;
        }
    }
}