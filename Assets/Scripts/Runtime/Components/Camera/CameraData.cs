using Unity.Cinemachine;
using UnityEngine;

namespace Universal.Runtime.Components.Camera
{
    [CreateAssetMenu(menuName = "Data/Camera/Config")]
    public class CameraData : ScriptableObject
    {
        [Header("Movement Settings")]
        [MinMaxRangeSlider(-60f, 60f)] public Vector2 verticalClamp = new(-60f, 60f);
        [MinMaxRangeSlider(-90f, 180f)] public Vector2 horizontalClamp = new(-90f, 90f);
        [MinMaxRangeSlider(0f, 10f)] public Vector2 sensitivityAmount = new(10f, 10f);
        [MinMaxRangeSlider(0f, 10f)] public Vector2 smoothAmount = new(10f, 10f);

        [Header("Recenter Settings")]
        [Min(0f)] public float recenterDuration = 0.5f;

        [Header("Aiming Settings")]
        [Range(20f, 60f)] public float zoomFOV = 20f;
        public AnimationCurve zoomCurve = new();
        public float zoomTransitionDuration = 0f;
        [Range(60f, 100f)] public float runFOV = 60f;
        public AnimationCurve runCurve = new();
        public float runTransitionDuration = 0f;
        public float runReturnTransitionDuration = 0f;

        [Header("Swaying Settings")]
        public float swayAmount = 0f;
        public float swaySpeed = 0f;
        public float returnSpeed = 0f;
        public float changeDirectionMultiplier = 0f;
        public AnimationCurve swayCurve = new();

        [Header("Breathing Settings")]
        public bool x = true;
        public bool y = true;
        public bool z = true;
    }
}