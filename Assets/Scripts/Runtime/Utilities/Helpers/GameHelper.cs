using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class GameHelper
    {
        public static float VectorHorizontalDistanceSqr(Vector3 a, Vector3 b)
        {
            var dx = a.x - b.x;
            var dz = a.z - b.z;
            return dx * dx + dz * dz;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            angle %= 360;
            if ((angle >= -360f) && (angle <= 360f))
            {
                if (angle < -360f)
                    angle += 360f;
                if (angle > 360f)
                    angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}