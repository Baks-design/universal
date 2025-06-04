namespace Universal.Runtime.Systems.WeatherAmbient
{
    public interface IWeatherState
    {
        void ApplyWeather();
        string GetWeatherName();
    }
}