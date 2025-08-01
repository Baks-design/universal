using UnityEngine;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class SpawnManager : MonoBehaviour, ISpawnServices
    {
        readonly List<ISpawnPoint> spawnPoints = new();
        ISpawnStrategy spawnStrategy;

        void Awake()
        {
            ServiceLocator.Global.Register<ISpawnServices>(this);
            spawnStrategy = new RandomSpawnStrategy();
        }

        public ISpawnPoint GetSpawnPoint()
        {
            var spawnPoint = spawnStrategy.SelectSpawnPoint(spawnPoints);
            if (spawnPoint == null)
            {
                Logging.LogError("No available spawn points!");
                return null;
            }

            spawnPoint.IsAvailable = false;

            return spawnPoint;
        }

        public void RegisterSpawnPoint(ISpawnPoint spawnPoint)
        {
            if (!spawnPoints.Contains(spawnPoint))
                spawnPoints.Add(spawnPoint);
        }

        public void UnregisterSpawnPoint(ISpawnPoint spawnPoint)
        {
            if (spawnPoints.Contains(spawnPoint))
                spawnPoints.Remove(spawnPoint);
        }
    }
}