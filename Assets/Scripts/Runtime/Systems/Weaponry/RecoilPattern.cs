using System;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    [Serializable]
    public class RecoilPattern
    {
        public float patternScale = 1f;
        public int currentIndex = 0;
        public Vector2[] patternPoints;

        public Vector2 GetCurrentPoint() => patternPoints[currentIndex] * patternScale;

        public void AdvancePattern() => currentIndex = (currentIndex + 1) % patternPoints.Length;

        public void ResetPattern() => currentIndex = 0;
    }
}
