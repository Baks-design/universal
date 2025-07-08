namespace Universal.Runtime.Behaviours.Characters
{
    public interface IMovementCommand
    {
        void Execute(IGridMover mover);
        bool CanExecute(IGridMover mover);
    }
}