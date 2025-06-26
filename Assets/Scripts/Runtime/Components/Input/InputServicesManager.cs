using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputServicesManager : MonoBehaviour, IInputServices
    {
        [SerializeField, Self] PlayerInputReader playerInput;
        [SerializeField, Self] UIInputReader uiInput;
        Gamepad gamepad;
        Coroutine currentRumbleRoutine;

        public GameInputs GameInputs { get; private set; }

        #region Initialization
        void Awake()
        {
            ServiceLocator.Global.Register<IInputServices>(this);
            DontDestroyOnLoad(gameObject);
            SetupActions();
            SetCursorLocked(true);
            UpdateGamepadReference();
        }

        void OnEnable() => InputSystem.onDeviceChange += OnDeviceChange;

        void OnDisable() => InputSystem.onDeviceChange -= OnDeviceChange;

        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change is not InputDeviceChange.Added &&
                change is not InputDeviceChange.Removed)
                return;

            UpdateGamepadReference();
        }

        void UpdateGamepadReference() => gamepad = Gamepad.current;
        #endregion

        #region Public Methods
        public void SetCursorLocked(bool isSet)
        {
            Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isSet;
        }
        #endregion

        #region Input Actions Management
        void SetupActions()
        {
            GameInputs = new GameInputs();
            GameInputs.Player.SetCallbacks(playerInput);
            GameInputs.UI.SetCallbacks(uiInput);
        }

        public void ChangeToPlayerMap()
        {
            GameInputs.Player.Enable();
            GameInputs.UI.Disable();
        }

        public void ChangeToUIMap()
        {
            GameInputs.Player.Disable();
            GameInputs.UI.Enable();
        }

        public void DisableGameInput() => GameInputs.Disable();
        #endregion

        #region Gamepad Rumble
        public void PulseRumble(
            float lowFreq, float highFreq, int pulses, float pulseDuration, float pauseDuration)
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
        #endregion
    }
}