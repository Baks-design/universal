using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class QuaternionRotationAnimator : IMovementAnimator
    {
        readonly CharacterData data;
        Quaternion? startRotation;
        Quaternion? targetRotation;
        float progress;

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
            if (duration <= Epsilon) return;
            startRotation = start;
            targetRotation = end;
            CurrentRotation = start;
            progress = 0f;
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating) return;

            progress += Time.deltaTime * data.rotationDuration;
            progress = Clamp01(progress);
            CurrentRotation = Quaternion.Slerp(startRotation.Value, targetRotation.Value, progress);

            if (progress >= 1f) CompleteAnimation();
        }

        void CompleteAnimation()
        {
            CurrentRotation = targetRotation.Value;
            startRotation = null;
            targetRotation = null;
        }
    }
}