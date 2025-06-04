namespace Universal.Runtime.Systems.WeatherAmbient
{
    public abstract class WeatherState : IWeatherState
    {
        public abstract void ApplyWeather();
        public abstract string GetWeatherName();
    }
}