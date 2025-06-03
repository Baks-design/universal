namespace Universal.Runtime.Systems.WorldTendency
{
    public interface ITendencyEffect
    {
        void ApplyEffect(int worldIndex, TendencyState state);
    }
}