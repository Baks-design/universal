using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);
            if (angle > 180f) angle -= 360f;
            return Clamp(angle, min, max);
        }

        public static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        public static float ExpDecay(float a, float b, float dt, float decay = 16f)
        => b + (a - b) * Exp(-decay * dt);

        public static Vector2 ExpDecay(Vector2 a, Vector2 b, float dt, float decay = 16f)
        => b + (a - b) * Exp(-decay * dt);

        public static Vector3 ExpDecay(Vector3 a, Vector3 b, float dt, float decay = 16f)
        => b + (a - b) * Exp(-decay * dt);

        public static float InverseExpDecay(float currentValue, float startValue, float targetValue, float decay = 16f)
        {
            var ratio = (currentValue - targetValue) / (startValue - targetValue);
            ratio = Clamp(ratio, 0.0001f, 1f);
            var t = -Log(ratio) / decay;
            return Clamp01(t * decay);
        }
    }
}
