using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class WeatherAmbientManager : MonoBehaviour, IWeatherServices
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
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IWeatherServices>(this);
            weatherSystem = new WeatherSystem(new SunnyWeather());
            daytimeSystem = new DaytimeSystem(new DaytimeMorning());
        }

        void Start()
        {
            SetDaytime(new DaytimeMorning());
            SetWeather(new RainyWeather());
            Debug.Log(GetCurrentEnvironmentInfo);
        }

        void Update() => daytimeSystem.UpdateDaytime();

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