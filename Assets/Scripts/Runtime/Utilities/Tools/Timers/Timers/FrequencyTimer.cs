using System;
using UnityEngine;

namespace Universal.Runtime.Utilities.Tools.Timers
{
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : Timer
    {
        float timeThreshold;

        public int TicksPerSecond { get; private set; }

        public Action OnTick = delegate { };

        public FrequencyTimer(int ticksPerSecond) : base(0) => CalculateTimeThreshold(ticksPerSecond);

        public override void Tick()
        {
            if (IsRunning && CurrentTime >= timeThreshold)
            {
                CurrentTime -= timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < timeThreshold)
                CurrentTime += Time.deltaTime;
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset() => CurrentTime = 0f;

        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}