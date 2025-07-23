using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IMovementInputProvider
    {
        Vector2 InputVector { get; }
        bool HasInput { get; }
        bool IsRunning { get; }
        bool IsCrouching { get; }
        bool RunClicked { get; }
        bool RunReleased { get; }
        bool CrouchClicked { get; }
        bool JumpClicked { get; }
    }
}