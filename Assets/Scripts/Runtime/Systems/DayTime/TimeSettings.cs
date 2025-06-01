using UnityEngine;

namespace Universal.Runtime.Systems.DayTime
{
    [CreateAssetMenu(menuName = "Data/DayTime/TimeSettings")]
    public class TimeSettings : ScriptableObject
    {
        public float timeMultiplier = 2000f;
        public float startHour = 12f;
        public float sunriseHour = 6f;
        public float sunsetHour = 18f;
    }
}