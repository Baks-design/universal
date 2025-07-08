using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class TurnLeftCommand : ITurnCommand
    {
        public float TurnAngle { get; } = -90f;

        public bool CanExecute(IGridMover mover)
        => !mover.IsMoving && !mover.IsRotating;

        public void Execute(IGridMover mover)
        {
            if (!CanExecute(mover)) return;
            mover.Rotation *= Quaternion.Euler(0f, -TurnAngle, 0f);
        }
    }
}