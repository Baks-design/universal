using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface ISpawnPoint
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        bool IsAvailable { get; set; }
    }
}