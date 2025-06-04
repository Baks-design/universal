namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class SunnyWeather : WeatherState
    {
        public override string GetWeatherName() => "Sunny";
        public override void ApplyWeather() { }
    }
}