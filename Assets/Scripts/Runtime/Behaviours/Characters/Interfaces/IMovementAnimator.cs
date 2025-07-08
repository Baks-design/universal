using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IMovementAnimator
    {
        bool IsAnimating { get; }
        Vector3 CurrentPosition { get; }
        Quaternion CurrentRotation { get; }

        void AnimateMovement(Vector3 start, Vector3 end, float duration) { }
        void AnimateRotation(Quaternion start, Quaternion end, float duration) { }
        void UpdateAnimation();
    }
}