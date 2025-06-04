namespace Universal.Runtime.Systems.WeatherAmbient
{
    public abstract class DaytimeState : IDaytimeState
    {
        protected float currentHour;

        public abstract void ApplyDaytime();
        public abstract string GetDaytimeName();

        public float GetCurrentHour() => currentHour;

        protected void IncrementTime(float deltaHours) => currentHour = (currentHour + deltaHours) % 24f;
    }
}