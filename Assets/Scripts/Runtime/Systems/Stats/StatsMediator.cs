using System.Collections.Generic;
using System.Linq;

namespace Universal.Runtime.Systems.Stats
{
    public class StatsMediator
    {
        readonly List<StatModifier> listModifiers = new();
        readonly Dictionary<StatType, IEnumerable<StatModifier>> modifiersCache = new();
        readonly IStatModifierApplicationOrder order = new NormalStatModifierOrder(); 

        public void PerformQuery(Query query)
        {
            if (!modifiersCache.ContainsKey(query.StatType))
                modifiersCache[query.StatType] = listModifiers.Where(modifier => modifier.Type == query.StatType).ToList();
            query.Value = order.Apply(modifiersCache[query.StatType], query.Value);
        }

        void InvalidateCache(StatType statType) => modifiersCache.Remove(statType);

        public void AddModifier(StatModifier modifier)
        {
            listModifiers.Add(modifier);
            InvalidateCache(modifier.Type);
            modifier.MarkedForRemoval = false;

            modifier.OnDispose += _ => InvalidateCache(modifier.Type);
            modifier.OnDispose += _ => listModifiers.Remove(modifier);
        }

        public void Update()
        {
            for (var i = 0; i < listModifiers.Count; i++)
                listModifiers[i].Update();

            var list = listModifiers.Where(modifier => modifier.MarkedForRemoval).ToList();
            for (var i = 0; i < list.Count; i++)
                list[i].Dispose();
        }
    }
}