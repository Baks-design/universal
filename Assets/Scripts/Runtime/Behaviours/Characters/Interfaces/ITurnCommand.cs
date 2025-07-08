namespace Universal.Runtime.Behaviours.Characters
{
    public interface ITurnCommand : IMovementCommand
    {
        float TurnAngle { get; }
    }
}