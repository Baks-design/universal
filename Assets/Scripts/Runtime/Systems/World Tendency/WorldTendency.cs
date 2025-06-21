using System;
using System.Collections.Generic;
using UnityEngine;
using Universal.Runtime.Systems.EntitiesPersistence;

namespace Universal.Runtime.Systems.WorldTendency
{
    public class WorldTendency : ITendencyNotifier //TODO: Adjust World Tendency
    {
        readonly ITendencyCalculator calculator;
        readonly ITendencyPersister persister;
        readonly List<ITendencyEffect> effects = new();
        float[] worldTendencies;

        public event Action<int, TendencyState> OnTendencyChanged = delegate { };

        public WorldTendency(ITendencyCalculator calculator, ITendencyPersister persister, int worldCount)
        {
            this.calculator = calculator;
            this.persister = persister;

            InitializeTendencies(worldCount);
        }

        void InitializeTendencies(int worldCount)
        {
            worldTendencies = persister.LoadTendencies();
            if (worldTendencies == null || worldTendencies.Length != worldCount)
            {
                worldTendencies = new float[worldCount];
                new GameData { worldCount = worldCount };
            }
        }

        public void AddEffect(ITendencyEffect effect) => effects.Add(effect);

        public void AdjustTendency(int worldIndex, float amount)
        {
            if (worldIndex < 0 || worldIndex >= worldTendencies.Length) return;

            var newValue = Mathf.Clamp(worldTendencies[worldIndex] + amount, -1f, 1f);
            if (Mathf.Abs(newValue - worldTendencies[worldIndex]) > 0.01f)
            {
                worldTendencies[worldIndex] = newValue;
                UpdateWorld(worldIndex);
            }
        }

        void UpdateWorld(int worldIndex)
        {
            var state = calculator.CalculateTendency(worldTendencies[worldIndex]);

            for (var i = 0; i < effects.Count; i++)
                effects[i].ApplyEffect(worldIndex, state);

            OnTendencyChanged.Invoke(worldIndex, state);
        }

        public void Save() => persister.SaveTendencies(worldTendencies);

        public TendencyState GetCurrentState(int worldIndex)
        => calculator.CalculateTendency(worldTendencies[worldIndex]);
    }
}