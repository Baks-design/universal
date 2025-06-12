using UnityEngine;

namespace Universal.Runtime.Systems.SoundEffects
{
    public enum SurfaceType
    {
        Rock,
        Tile,
        Wood
    }

    public class SurfaceTag : MonoBehaviour
    {
        [field: SerializeField] public SurfaceType SurfaceType { get; private set; }
    }
}
