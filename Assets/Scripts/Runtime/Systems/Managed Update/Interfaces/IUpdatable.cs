namespace Universal.Runtime.Systems.ManagedUpdate
{
    public interface IUpdatable : IManagedObject
    {
        void ManagedUpdate(float deltaTime, float time);
    }
}
