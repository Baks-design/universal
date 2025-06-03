using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public class WeatherSystem : MonoBehaviour, IWeatherSystem
    {
        [SerializeField, Child] List<WeatherEffectHandler> effectHandlers;
        WeatherState currentWeather;

        public WeatherState CurrentWeather => currentWeather;

        public void SetCurrentWeather(WeatherState newState)
        {
            if (currentWeather == newState)
                return;
                
            currentWeather = newState;
            NotifyHandlers();
        }

        void NotifyHandlers()
        {
            for (var i = 0; i < effectHandlers.Count; i++)
                effectHandlers[i].HandleWeatherChange(currentWeather);
        }
    }
}