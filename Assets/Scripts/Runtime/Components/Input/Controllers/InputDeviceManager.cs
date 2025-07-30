using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputDeviceManager : MonoBehaviour, IInputDeviceServices
    {
        InputDevice lastActiveDevice;

        public InputDevice LastActiveDevice => lastActiveDevice;
        public bool IsGamepadLastActiveDevice => lastActiveDevice is Gamepad;

        void Awake() => ServiceLocator.Global.Register<IInputDeviceServices>(this);
        
        void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onActionChange += OnActionChange;
        }

        void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onActionChange -= OnActionChange;
        }

        void Start() => UpdateActiveDevice(Keyboard.current != null ? Keyboard.current : (Gamepad.current ?? null));

        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:

                    Logging.Log($"Device connected: {device.name}");
                    if (device is Gamepad)
                        UpdateActiveDevice(device);

                    break;

                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:

                    Logging.Log($"Device disconnected: {device.name}");
                    if (device == lastActiveDevice)
                        UpdateActiveDevice(Keyboard.current);

                    break;
            }
        }

        void OnActionChange(object action, InputActionChange change)
        {
            // This helps detect when input switches between devices
            if (change is InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction)action;
                var device = inputAction.activeControl.device;

                // Only switch if it's a significant device change
                if ((device is Gamepad && lastActiveDevice is not Gamepad) ||
                        ((device is Keyboard || device is Mouse) &&
                            lastActiveDevice is Gamepad))
                    UpdateActiveDevice(device);
            }
        }

        void UpdateActiveDevice(InputDevice device)
        {
            if (device == null) return;

            lastActiveDevice = device;

            if (device is Gamepad)
                Logging.Log("Switched to Gamepad controls");
            else if (device is Keyboard || device is Mouse)
                Logging.Log("Switched to Keyboard/Mouse controls");
        }
    }
}