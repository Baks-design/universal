using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.AmbientWeather
{
    public class WeatherManager : PersistentSingleton<WeatherManager>
    {
        public WeatherState CurrentWeather { get; set; }

        public void SetWeather(WeatherState newState) => CurrentWeather = newState;
    }
}