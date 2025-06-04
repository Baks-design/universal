namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class WeatherSystem
    {
        IWeatherState currentWeather;

        public string GetCurrentWeather => currentWeather.GetWeatherName();

        public WeatherSystem(IWeatherState initialState) => currentWeather = initialState;

        public void SetWeather(IWeatherState newWeather)
        {
            currentWeather = newWeather;
            currentWeather.ApplyWeather();
        }
    }
}