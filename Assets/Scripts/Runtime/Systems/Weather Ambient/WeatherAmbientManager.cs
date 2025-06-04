using Universal.Runtime.Systems.ManagedUpdate;
using UnityEngine;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class WeatherAmbientManager : PersistentSingleton<WeatherAmbientManager>, IUpdatable
    {
        WeatherSystem weatherSystem;
        DaytimeSystem daytimeSystem;

        public string GetCurrentEnvironmentInfo
        {
            get
            {
                var daytime = daytimeSystem.GetCurrentDaytime;
                var currentHour = daytimeSystem.GetCurrentHour;
                var weather = weatherSystem.GetCurrentWeather;
                return $"Current: {daytime} ({currentHour:F1}h), {weather}";
            }
        }
        protected override void Awake()
        {
            base.Awake();
            weatherSystem = new WeatherSystem(new SunnyWeather());
            daytimeSystem = new DaytimeSystem(new DaytimeMorning());
        }

        void Start()
        {
            SetDaytime(new DaytimeMorning());
            SetWeather(new RainyWeather());
            Debug.Log(GetCurrentEnvironmentInfo);
        }

        void IUpdatable.ManagedUpdate(float deltaTime, float time) => daytimeSystem.UpdateDaytime();

        public void SetWeather(IWeatherState newWeather)
        {
            weatherSystem.SetWeather(newWeather);
            Debug.Log(GetCurrentEnvironmentInfo);
        }

        public void SetDaytime(IDaytimeState newDaytime)
        {
            daytimeSystem.SetDaytime(newDaytime);
            Debug.Log(GetCurrentEnvironmentInfo);
        }
    }
}