using Unity.Cinemachine;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Movement Settings")]
        [MinMaxRangeSlider(-85f, 85f)] public Vector2 verticalClamp = new(-60f, 60f);
        [MinMaxRangeSlider(-180f, 180f)] public Vector2 horizontalClamp = new(-90f, 90f);
        [MinMaxRangeSlider(0.1f, 10f)] public Vector2 sensitivityAmount = new(2f, 2f);
        [MinMaxRangeSlider(0f, 10f)] public Vector2 smoothAmount = new(3f, 3f);

        [Header("Recenter Settings")]
        [Min(0f)] public float recenterDuration = 3f;

        [Header("Aiming Settings")]
        [Range(20f, 60f)] public float zoomFOV = 40f;
        public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float zoomTransitionDuration = 0.3f;
        [Range(60f, 100f)] public float runFOV = 75f;
        public AnimationCurve runCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float runTransitionDuration = 0.5f;
        public float runReturnTransitionDuration = 1f;

        [Header("Swaying Settings")]
        public float swayAmount = 0.1f; 
        public float swaySpeed = 1f; 
        public float returnSpeed = 2f; 
        public float changeDirectionMultiplier = 1.5f;
        public AnimationCurve swayCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}