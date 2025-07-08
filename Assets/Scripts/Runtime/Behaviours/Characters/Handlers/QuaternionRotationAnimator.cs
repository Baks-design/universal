using UnityEngine;

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
            startRotation = start;
            targetRotation = end;
            CurrentRotation = start;
            progress = 0f;
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating) return;

            progress += Time.deltaTime * data.rotationSpeed;
            CurrentRotation = Quaternion.Slerp(startRotation.Value, targetRotation.Value, progress);

            if (progress >= 1f)
            {
                CurrentRotation = targetRotation.Value;
                startRotation = targetRotation = null;
            }
        }
    }
}