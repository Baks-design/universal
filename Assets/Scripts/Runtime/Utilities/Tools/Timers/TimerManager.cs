using System.Collections.Generic;

namespace Universal.Runtime.Utilities.Tools.Timers
{
    public static class TimerManager
    {
        static readonly List<Timer> timers = new();
        static readonly List<Timer> sweep = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static void UpdateTimers()
        {
            if (timers.Count == 0)
                return;

            sweep.RefreshWith(timers);
            for (var i = 0; i < sweep.Count; i++)
                sweep[i].Tick();
        }

        public static void Clear()
        {
            sweep.RefreshWith(timers);
            for (var i = 0; i < sweep.Count; i++)
                sweep[i].Dispose();

            timers.Clear();
            sweep.Clear();
        }
    }
}