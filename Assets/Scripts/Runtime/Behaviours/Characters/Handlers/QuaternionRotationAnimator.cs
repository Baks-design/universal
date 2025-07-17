using UnityEngine;
using static Freya.Mathfs;
using Random = Freya.Random;

namespace Universal.Runtime.Behaviours.Characters
{
    public class QuaternionRotationAnimator : IMovementAnimator
    {
        readonly CharacterData data;
        Quaternion? startRotation;
        Quaternion? targetRotation;
        Quaternion overshootRotation;
        float progress;
        float overshootProgress;
        bool isOvershooting;

        public bool IsAnimating => startRotation.HasValue;
        public Vector3 CurrentPosition => Vector3.zero;
        public Quaternion CurrentRotation { get; private set; }

        public QuaternionRotationAnimator(CharacterData data)
        {
            this.data = data;
            CurrentRotation = Quaternion.identity;
        }

        public void AnimateRotation(Quaternion start, Quaternion end, float duration)
        {
            if (duration <= Epsilon)
            {
                SnapToRotation(end);
                return;
            }

            startRotation = start;
            targetRotation = end;
            CurrentRotation = start;
            progress = 0f;
            isOvershooting = false;

            var angle = Quaternion.Angle(start, end);
            data.rotationDuration = duration * (angle / data.angleNormalizationBase);
        }

        void SnapToRotation(Quaternion rotation)
        {
            CurrentRotation = rotation;
            startRotation = null;
            targetRotation = null;
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating || startRotation == null || targetRotation == null)
                return;

            var remainingAngle = Quaternion.Angle(CurrentRotation, targetRotation.Value);

            var vel = remainingAngle / data.angularSpeedThreshold;
            var speedMultiplier = Clamp(vel, data.minSpeedMultiplier, data.maxSpeedMultiplier);

            progress += Time.deltaTime * (1f / data.rotationDuration * speedMultiplier);
            var time = EvaluateCurve(progress);

            if (ShouldTriggerOvershoot())
                InitializeOvershoot();

            if (isOvershooting)
                UpdateOvershootRotation(time);
            else
                CurrentRotation = Quaternion.Slerp(startRotation.Value, targetRotation.Value, time);

            if (progress >= 1f)
                CompleteAnimation();
        }

        float EvaluateCurve(float t) => data.rotationCurve.Evaluate(Clamp01(t));

        bool ShouldTriggerOvershoot()
        => data.enableOvershoot &&
            progress >= data.overshootMinTriggerProgress &&
            !isOvershooting &&
            Random.Value <= data.overshootChance;

        void InitializeOvershoot()
        {
            isOvershooting = true;
            var randomOvershoot = new Vector3(0f, data.overshootAmount.y * Random.Range(-1f, 1f), 0f);
            overshootRotation = targetRotation.Value * Quaternion.Euler(randomOvershoot);
            overshootProgress = 0f;
        }

        void UpdateOvershootRotation(float time)
        {
            overshootProgress += Time.deltaTime * data.overshootRecoverySpeed;
            var initRotation = Quaternion.Slerp(startRotation.Value, overshootRotation, time);
            CurrentRotation = Quaternion.Slerp(initRotation, targetRotation.Value, overshootProgress);
        }

        void CompleteAnimation()
        {
            CurrentRotation = targetRotation.Value;
            startRotation = null;
            targetRotation = null;
            isOvershooting = false;
        }
    }
}