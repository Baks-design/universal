namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class GridTurnCommand : IMovementCommand
    {
        public virtual float TurnAngle { get; }
        
        public virtual bool CanExecute(IGridMover mover)
        => !mover.IsMoving && !mover.IsRotating;

        public virtual void Execute(IGridMover mover)
        {
            if (!CanExecute(mover)) return;
        }
    }
}