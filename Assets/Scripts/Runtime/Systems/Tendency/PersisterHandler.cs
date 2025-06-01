using UnityEngine;
using Universal.Runtime.Systems.Persistence.Data;
using Universal.Runtime.Systems.Tendency.Interfaces;

namespace Universal.Runtime.Systems.Tendency
{
    public class PersisterHandler : ITendencyPersister
    {
        readonly GameData gameData = new();

        public void SaveTendencies(float[] tendencies)
        {
            for (var i = 0; i < tendencies.Length; i++)
                gameData.globalTendency = tendencies[i];
        }

        public float[] LoadTendencies()
        {
            var tendencies = new float[gameData.worldCount];
            for (var i = 0; i < gameData.worldCount; i++)
                tendencies[i] = gameData.globalTendency;
            return tendencies;
        }
    }
}