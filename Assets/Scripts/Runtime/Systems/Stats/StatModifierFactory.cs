using System;

namespace Universal.Runtime.Systems.Stats
{
    public class StatModifierFactory : IStatModifierFactory
    {
        public StatModifier Create(OperatorType operatorType, StatType statType, int value, float duration)
        {
            IOperationStrategy strategy = operatorType switch
            {
                OperatorType.Add => new AddOperation(value),
                OperatorType.Multiply => new MultiplyOperation(value),
                _ => throw new ArgumentOutOfRangeException()
            };

            return new StatModifier(statType, strategy, duration);
        }
    }
}