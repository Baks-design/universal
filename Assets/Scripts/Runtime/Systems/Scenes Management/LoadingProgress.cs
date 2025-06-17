using System;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public class LoadingProgress : IProgress<float>
    {
        const float ratio = 1f;

        public event Action<float> Progressed = delegate { };

        public void Report(float value) => Progressed.Invoke(value / ratio);
    }
}