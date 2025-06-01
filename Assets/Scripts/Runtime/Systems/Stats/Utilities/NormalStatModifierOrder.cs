using System.Collections.Generic;
using System.Linq;

namespace Universal.Runtime.Systems.Stats
{
    public class NormalStatModifierOrder : IStatModifierApplicationOrder
    {
        public int Apply(IEnumerable<StatModifier> statModifiers, int baseValue)
        {
            // Convert to list only once (avokes multiple enumeration)
            var allModifiers = new List<StatModifier>(statModifiers);

            // Process AddOperations first
            for (var i = 0; i < allModifiers.Count; i++)
            {
                var modifier = allModifiers[i];
                if (modifier.Strategy is AddOperation)
                    baseValue = modifier.Strategy.Calculate(baseValue);
            }

            // Then process MultiplyOperations
            for (var i = 0; i < allModifiers.Count; i++)
            {
                var modifier = allModifiers[i];
                if (modifier.Strategy is MultiplyOperation)
                    baseValue = modifier.Strategy.Calculate(baseValue);
            }

            return baseValue;
        }
    }
}