using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class AMathfs
    {
        /// <summary>
        /// 1 - Forward /
        /// 2 - Back /
        /// 3 - Left /
        /// 4 - Right 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Vector3Int SetDirection(int dir) => dir switch
        {
            1 => new Vector3Int(0, 1, 0),
            2 => new Vector3Int(0, -1, 0),
            3 => Vector3Int.left,
            4 => Vector3Int.right,
            _ => new Vector3Int(0, 0, 0),
        };
        
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