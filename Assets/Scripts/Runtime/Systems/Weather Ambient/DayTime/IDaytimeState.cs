namespace Universal.Runtime.Systems.WeatherAmbient
{
    public interface IDaytimeState
    {
        void ApplyDaytime();
        string GetDaytimeName();
        float GetCurrentHour();
    }
}