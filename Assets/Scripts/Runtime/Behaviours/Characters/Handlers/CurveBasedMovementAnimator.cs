using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CurveBasedMovementAnimator : IMovementAnimator
    {
        readonly CharacterData data;
        Vector3? startPosition;
        Vector3? targetPosition;
        float progress;

        public bool IsAnimating => startPosition.HasValue;
        public Quaternion CurrentRotation => Quaternion.identity;
        public Vector3 CurrentPosition { get; private set; }

        public CurveBasedMovementAnimator(CharacterData data) => this.data = data;

        public void AnimateMovement(Vector3 start, Vector3 end, float duration)
        {
            startPosition = start;
            targetPosition = end;
            CurrentPosition = start;
            progress = 0f;
        }

        public void UpdateAnimation()
        {
            if (!IsAnimating) return;

            progress += Time.deltaTime * data.movementSpeed;
            var t = data.movementCurve.Evaluate(progress);
            CurrentPosition = Vector3.Lerp(startPosition.Value, targetPosition.Value, t);

            if (progress >= 1f)
            {
                CurrentPosition = targetPosition.Value;
                startPosition = targetPosition = null;
            }
        }
    }
}