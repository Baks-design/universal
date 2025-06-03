using UnityEngine;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public class WeatherController : MonoBehaviour
    {
        readonly IWeatherSystem weatherSystem;

        public WeatherController(IWeatherSystem weatherSystem) => this.weatherSystem = weatherSystem;

        public void ChangeWeather(WeatherState newState) => weatherSystem.SetCurrentWeather(newState);
    }
}