using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class GridMovementCommand : IMovementCommand
    {
        public abstract Vector3 GetMovementDirection(IGridMover mover);

        public virtual bool CanExecute(IGridMover mover)
        => !mover.IsMoving && !mover.IsRotating;

        public virtual void Execute(IGridMover mover)
        {
            if (!CanExecute(mover)) return;
            var direction = GetMovementDirection(mover);
            mover.Position += direction * mover.GridSize;
        }
    }
}