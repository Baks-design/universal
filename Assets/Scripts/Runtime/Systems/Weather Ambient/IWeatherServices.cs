namespace Universal.Runtime.Systems.WeatherAmbient
{
    public interface IWeatherServices
    {
        void SetWeather(IWeatherState newWeather);
        void SetDaytime(IDaytimeState newDaytime);
    }
}