using UnityEngine;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public interface IPhysicalWeatherEffect
    {
        void ApplyPhysicsEffects(Rigidbody rb);
    }
}