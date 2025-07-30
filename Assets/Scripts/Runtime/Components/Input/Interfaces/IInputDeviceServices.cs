using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public interface IInputDeviceServices
    {
        public InputDevice LastActiveDevice { get; }
        public bool IsGamepadLastActiveDevice { get; }
    }
}