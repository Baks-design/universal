namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class DaytimeSystem
    {
        IDaytimeState currentDaytime;

        public string GetCurrentDaytime => currentDaytime.GetDaytimeName();
        public float GetCurrentHour => currentDaytime.GetCurrentHour();

        public DaytimeSystem(IDaytimeState initialState) => currentDaytime = initialState;

        public void SetDaytime(IDaytimeState newDaytime)
        {
            currentDaytime = newDaytime;
            currentDaytime.ApplyDaytime();
        }

        public void UpdateDaytime() => currentDaytime.ApplyDaytime();
    }
}