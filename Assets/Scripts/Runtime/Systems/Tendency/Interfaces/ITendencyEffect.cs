namespace Universal.Runtime.Systems.Tendency.Interfaces
{
    public interface ITendencyEffect
    {
        void ApplyEffect(int worldIndex, TendencyState state);
    }
}