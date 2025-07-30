namespace Universal.Runtime.Utilities.Tools.Updates
{
    public interface IConditionalUpdatable : IUpdatable
    {
        bool ShouldUpdate();
    }
}