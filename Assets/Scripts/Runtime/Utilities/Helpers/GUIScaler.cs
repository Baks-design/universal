using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class GUIScaler
    {
        const float REF_WIDTH = 1920f;
        const float REF_HEIGHT = 1080f;
        const float REF_ASPECT = REF_WIDTH / REF_HEIGHT;

        public static void BeginScaledGUI()
        {
            var currentAspect = (float)Screen.width / Screen.height;
            var scaleFactor = currentAspect > REF_ASPECT ? Screen.height / REF_HEIGHT : Screen.width / REF_WIDTH;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scaleFactor, scaleFactor, 1f));
        }

        public static Vector2 ScalePosition(Vector2 position)
        {
            var scale = GetCurrentScale();
            return new Vector2(position.x * scale, position.y * scale);
        }

        public static void DrawProportionalLabel(Vector2 position, string text, GUIStyle style)
        {
            var scale = GetCurrentScale();
            var scaledPos = new Vector2(position.x * scale, position.y * scale);
            GUI.Label(new Rect(scaledPos.x, scaledPos.y, 1000f, 100f), text, style);
        }

        public static float GetCurrentScale()
        {
            var currentAspect = (float)Screen.width / Screen.height;
            return currentAspect > REF_ASPECT ? Screen.height / REF_HEIGHT : Screen.width / REF_WIDTH;
        }
    }
}
