namespace Universal.Runtime.Components.Input
{
    public interface IInputServices
    {
        void SetCursorLocked(bool isSet);
        void ChangeToPlayerMap();
        void ChangeToUIMap();
        void DisableGameInput();
        void PulseRumble(
            float lowFreq, float highFreq, int pulses,
            float pulseDuration, float pauseDuration);
        void RampRumble(float duration, bool rampUp);
    }
}