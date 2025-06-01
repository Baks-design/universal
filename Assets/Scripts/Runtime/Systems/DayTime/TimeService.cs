using System;
using UnityEngine;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.DayTime
{
    public class TimeService
    {
        DateTime currentTime;
        readonly TimeSettings settings;
        readonly TimeSpan sunriseTime;
        readonly TimeSpan sunsetTime;

        public DateTime CurrentTime => currentTime;
        // This Property checks whether the current game time falls within the daytime period.
        // It returns true if the current time of day is later than sunriseTime and earlier than sunsetTime,
        // representing daytime. Otherwise, it returns false, indicating it is nighttime.
        public bool IsDayTime => currentTime.TimeOfDay >= sunriseTime && currentTime.TimeOfDay <= sunsetTime;

        public event Action OnSunrise = delegate { };
        public event Action OnSunset = delegate { };
        public event Action OnHourChange = delegate { };
        readonly Observable<bool> isDayTime;
        readonly Observable<int> currentHour;

        public TimeService(TimeSettings settings)
        {
            this.settings = settings;

            currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
            sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
            sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

            isDayTime = new Observable<bool>(IsDayTime);
            currentHour = new Observable<int>(currentTime.Hour);

            isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
            currentHour.ValueChanged += _ => OnHourChange?.Invoke();
        }

        public void UpdateTime(float deltaTime)
        {
            currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
            isDayTime.Value = IsDayTime;
            currentHour.Value = currentTime.Hour;
        }

        public float CalculateSunAngle()
        {
            var isDay = IsDayTime;
            var startDegree = isDay ? 0 : 180;
            var start = isDay ? sunriseTime : sunsetTime;
            var end = isDay ? sunsetTime : sunriseTime;

            var totalTime = CalculateDifference(start, end);
            var elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

            var percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
            return Mathf.Lerp(startDegree, startDegree + 180f, (float)percentage);
        }

        // This method calculates the difference between two TimeSpan objects ("from" and "to").
        // If the calculated difference is negative, this indicates that the "from" time is ahead of the "to" time.
        // In such cases, 24 hours (representing a full day) is added to the negative difference to calculate the actual
        // time difference taking into account the next day.    
        TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
        {
            var difference = to - from;
            return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
        }
    }
}