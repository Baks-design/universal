using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Camera Movement Settings")]
        [MinMaxSlider(-60f, 60f)] public Vector2 verticalClamp = new(-60f, 60f);
        [MinMaxSlider(-90f, 180f)] public Vector2 horizontalClamp = new(-90f, 90f);
        [MinMaxSlider(0f, 10f)] public Vector2 sensitivityAmount = new(10f, 10f);
        [MinMaxSlider(0f, 10f)] public Vector2 smoothAmount = new(10f, 10f);
        [Min(0f)] public float recenterDuration = 0.5f;
        [Min(0f)] public float recenterSpeed = 0.1f;
    }
}