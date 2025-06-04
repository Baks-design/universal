namespace Universal.Runtime.Systems.ManagedUpdate
{
    public interface ILateUpdatable : IManagedObject
    {
        void ManagedLateUpdate(float deltaTime, float time);
    }
}
