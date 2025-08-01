using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class Helpers
    {
        public static bool IsInLayerMask(int layer, LayerMask layerMask) => layerMask == (layerMask | (1 << layer));

        public static float ClampAngle(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);
            if (angle > 180f) angle -= 360f;
            return Clamp(angle, min, max);
        }

        static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            return angle < 0f ? angle + 360f : angle;
        }

        public static float ExpDecay(float current, float target, float deltaTime, float decay = 16f)
        => target + (current - target) * Exp(-decay * deltaTime);

        public static float HybridDecay(float current, float target, float deltaTime, float sharpness = 16f, float threshold = 0.5f)
        {
            var distance = Abs(target - current);
            if (distance > threshold)
                return InverseDecay(current, target, sharpness * 2f, deltaTime);
            else
                return ExpDecay(current, target, sharpness, deltaTime);
        }

        public static Vector2 ExpDecay(Vector2 current, Vector2 target, float deltaTime, float decay = 16f)
        => target + (current - target) * Exp(-decay * deltaTime);

        public static Vector3 ExpDecay(Vector3 current, Vector3 target, float deltaTime, float decay = 16f)
        => target + (current - target) * Exp(-decay * deltaTime);

        public static Quaternion ExpDecayRotation(Quaternion current, Quaternion target, float deltaTime, float decay = 16f)
        => Quaternion.Slerp(current, target, 1f - Exp(-decay * deltaTime));

        public static float InverseDecay(float current, float target, float deltaTime, float decay = 16f)
        => current + (target - current) * 1f - Exp(-decay * deltaTime);

        public static Vector3 InverseDecay(Vector3 current, Vector3 target, float deltaTime, float decay = 16f)
        => Vector3.Lerp(current, target, 1f - Exp(-decay * deltaTime));

        public static Quaternion InverseDecayRotation(Quaternion current, Quaternion target, float deltaTime, float decay = 16f)
        => Quaternion.Slerp(current, target, 1f - Exp(-decay * deltaTime));
    }
}