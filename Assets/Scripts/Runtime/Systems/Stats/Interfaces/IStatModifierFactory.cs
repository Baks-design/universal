namespace Universal.Runtime.Systems.Stats
{
    public interface IStatModifierFactory
    {
        StatModifier Create(OperatorType operatorType, StatType statType, int value, float duration);
    }
}