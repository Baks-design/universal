using System.Collections.Generic;
using Random = Freya.Random;

namespace Universal.Runtime.Behaviours.Characters
{
    public class RandomSpawnStrategy : ISpawnStrategy
    {
        public ISpawnPoint SelectSpawnPoint(IEnumerable<ISpawnPoint> spawnPoints)
        {
            var availableCount = 0;
            foreach (var point in spawnPoints)
                if (point.IsAvailable)
                    availableCount++;
            if (availableCount == 0) return null;

            var randomIndex = Random.Range(0, availableCount);
            var currentIndex = 0;
            foreach (var point in spawnPoints)
            {
                if (point.IsAvailable)
                {
                    if (currentIndex == randomIndex) return point;
                    currentIndex++;
                }
            }

            return null; 
        }
    }
}