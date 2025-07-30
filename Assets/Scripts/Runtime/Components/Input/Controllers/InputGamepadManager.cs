using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputGamepadManager : MonoBehaviour, IInputGamepadServices
    {
        readonly Gamepad gamepad;
        Coroutine currentRumbleRoutine;

        void Awake() => ServiceLocator.Global.Register<IInputGamepadServices>(this);

        public void PulseRumble(float lowFreq, float highFreq, int pulses, float pulseDuration, float pauseDuration)
        {
            StopAllRumbles();
            var pulseRoutine = PulseRumbleCoroutine(lowFreq, highFreq, pulses, pulseDuration, pauseDuration);
            currentRumbleRoutine = StartCoroutine(pulseRoutine);
        }

        public void RampRumble(float duration, bool rampUp)
        {
            StopAllRumbles();
            currentRumbleRoutine = StartCoroutine(RampRumbleCoroutine(duration, rampUp));
        }

        public void StopAllRumbles()
        {
            if (currentRumbleRoutine != null)
            {
                StopCoroutine(currentRumbleRoutine);
                currentRumbleRoutine = null;
            }

            gamepad.SetMotorSpeeds(0f, 0f);
        }

        IEnumerator PulseRumbleCoroutine(
            float lowFreq, float highFreq, int pulses,
            float pulseDuration, float pauseDuration)
        {
            try
            {
                for (var i = 0; i < pulses; i++)
                {
                    gamepad.SetMotorSpeeds(lowFreq, highFreq);
                    yield return Helpers.GetWaitForSeconds(pulseDuration);
                    gamepad.SetMotorSpeeds(0f, 0f);
                    yield return Helpers.GetWaitForSeconds(pauseDuration);
                }
            }
            finally
            {
                gamepad.SetMotorSpeeds(0f, 0f);
                currentRumbleRoutine = null;
            }
        }

        IEnumerator RampRumbleCoroutine(float duration, bool rampUp)
        {
            try
            {
                var timer = 0f;
                while (timer < duration)
                {
                    var intensity = rampUp ? (timer / duration) : (1f - (timer / duration));
                    gamepad.SetMotorSpeeds(intensity, intensity);
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            finally
            {
                gamepad.SetMotorSpeeds(0f, 0f);
                currentRumbleRoutine = null;
            }
        }
    }
}