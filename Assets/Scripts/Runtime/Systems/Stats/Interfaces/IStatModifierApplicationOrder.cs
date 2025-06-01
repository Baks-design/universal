using System.Collections.Generic;

namespace Universal.Runtime.Systems.Stats
{
    public interface IStatModifierApplicationOrder
    {
        int Apply(IEnumerable<StatModifier> statModifiers, int baseValue);
    }
}