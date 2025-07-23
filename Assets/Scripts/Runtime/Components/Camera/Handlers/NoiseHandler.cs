using UnityEngine;
using static Freya.Random;

namespace Universal.Runtime.Components.Camera
{
    public class NoiseHandler
    {
        readonly CameraSettings settings;
        Vector3 noiseOffset;
        Vector3 noise;

        public Vector3 Noise => noise;

        public NoiseHandler(CameraSettings settings)
        {
            this.settings = settings;

            var rand = 32f;

            noiseOffset.x = Range(0f, rand);
            noiseOffset.y = Range(0f, rand);
            noiseOffset.z = Range(0f, rand);
        }

        public void UpdateNoise()
        {
            var scrollOffset = Time.deltaTime * settings.frequency;

            noiseOffset.x += scrollOffset;
            noiseOffset.y += scrollOffset;
            noiseOffset.z += scrollOffset;

            noise.x = Mathf.PerlinNoise(noiseOffset.x, 0f);
            noise.y = Mathf.PerlinNoise(noiseOffset.x, 1f);
            noise.z = Mathf.PerlinNoise(noiseOffset.x, 2f);

            noise -= Vector3.one * 0.5f;
            noise *= settings.amplitude;
        }
    }
}