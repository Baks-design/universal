namespace Universal.Runtime.Systems.WeatherAmbient
{
    public class DaytimeNight : DaytimeState
    {
        public override void ApplyDaytime() => IncrementTime(0.1f);
        public override string GetDaytimeName() => "Night";
    }
}