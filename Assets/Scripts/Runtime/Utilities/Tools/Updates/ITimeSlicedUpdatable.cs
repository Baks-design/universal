namespace Universal.Runtime.Utilities.Tools.Updates
{
    public interface ITimeSlicedUpdatable : IUpdatable
    {
        int ExecutionOrder { get; }
        bool IsComplete { get; }

        void ResetSlice();
    }
}