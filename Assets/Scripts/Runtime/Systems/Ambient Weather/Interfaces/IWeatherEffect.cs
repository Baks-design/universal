namespace Universal.Runtime.Systems.AmbientWeather
{
    public interface IWeatherEffect
    {
        float Intensity { get; set; }

        void StartEffect();
        void StopEffect();
    }
}