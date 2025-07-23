using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IMovementHandler
    {
        bool IsGrounded { get; }
        Vector3 Velocity { get; }

        void HandleMovement();
    }
}