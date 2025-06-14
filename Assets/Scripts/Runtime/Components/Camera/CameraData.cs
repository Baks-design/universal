using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Camera Movement Settings")]
        [MinMaxSlider(-45f, 45f)] public Vector2 verticalClamp = new(-45f, 45f);
        [MinMaxSlider(-45f, 45f)] public Vector2 horizontalClamp = new(-45f, 45f);
        [MinMaxSlider(0f, 10f)] public Vector2 sensitivityAmount = new(10f, 10f);
        [MinMaxSlider(0f, 10f)] public Vector2 smoothAmount = new(10f, 10f);
    }
}