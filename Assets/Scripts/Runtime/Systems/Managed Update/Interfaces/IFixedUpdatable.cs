namespace Universal.Runtime.Systems.ManagedUpdate
{
    public interface IFixedUpdatable : IManagedObject
    {
        void ManagedFixedUpdate(float deltaTime, float time);
    }
}