using System;

namespace Universal.Runtime.Utilities.Tools.Updates
{
    [Serializable]
    public class FrameBudgetSettings
    {
        public float earlyUpdateMaxMs = 2f;
        public float normalUpdateMaxMs = 4f;
        public float lateUpdateMaxMs = 2f;
        public float uiUpdateMaxMs = 1f;
    }
}