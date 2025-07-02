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

        public static float EaseInOutQuad(float t)
        => t < 0.5f ? 2f * t * t : 1f - Pow(-2f * t + 2f, 2f) / 2f;
    }
}
