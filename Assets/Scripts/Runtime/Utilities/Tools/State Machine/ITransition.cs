namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}