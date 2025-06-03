namespace Universal.Runtime.Systems.AmbientWeather
{
    public interface IWeatherSystem
    {
        WeatherState CurrentWeather { get; }

        void SetCurrentWeather(WeatherState newState);
    }
}