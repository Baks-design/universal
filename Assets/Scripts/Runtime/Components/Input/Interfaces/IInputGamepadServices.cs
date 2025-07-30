namespace Universal.Runtime.Components.Input
{
    public interface IInputGamepadServices
    {
        void PulseRumble(float lowFreq, float highFreq, int pulses, float pulseDuration, float pauseDuration);
        void RampRumble(float duration, bool rampUp);
        void StopAllRumbles();
    }
}