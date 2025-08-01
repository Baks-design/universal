using System.Collections.Generic;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface ISpawnStrategy
    {
        ISpawnPoint SelectSpawnPoint(IEnumerable<ISpawnPoint> spawnPoints);
    }
}