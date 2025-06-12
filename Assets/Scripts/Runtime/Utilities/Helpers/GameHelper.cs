using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class GameHelper
    {
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