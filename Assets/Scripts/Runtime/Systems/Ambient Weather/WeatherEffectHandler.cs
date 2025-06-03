using UnityEngine;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public abstract class WeatherEffectHandler : MonoBehaviour
    {
        public abstract void HandleWeatherChange(WeatherState newState);
    }
}