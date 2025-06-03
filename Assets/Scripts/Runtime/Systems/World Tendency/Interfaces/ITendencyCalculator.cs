namespace Universal.Runtime.Systems.WorldTendency
{
    public interface ITendencyCalculator
    {
        TendencyState CalculateTendency(float tendencyValue);
    }
}