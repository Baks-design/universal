using System;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.Timers;

namespace Universal.Runtime.Systems.Stats
{
    public class StatModifier : IDisposable
    {
        public readonly Sprite icon;
        bool markedForRemoval;
        readonly CountdownTimer timer;

        public StatType Type { get; }
        public IOperationStrategy Strategy { get; }
        public bool MarkedForRemoval { get => markedForRemoval; set => markedForRemoval = value; }

        public event Action<StatModifier> OnDispose = delegate { };

        public StatModifier(StatType type, IOperationStrategy strategy, float duration)
        {
            Type = type;
            Strategy = strategy;
            if (duration <= 0f)
                return;

            timer = new CountdownTimer(duration);
            timer.OnTimerStop += () => markedForRemoval = true;
            timer.Start();
        }

        public void Update() => timer?.Tick();

        public void Handle(Query query)
        {
            if (query.StatType == Type)
                query.Value = Strategy.Calculate(query.Value);
        }

        public void Dispose() => OnDispose.Invoke(this);
    }
}