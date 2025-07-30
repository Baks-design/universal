using System;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class UIInputReader : MonoBehaviour, IUIActions, IUIInputReader
    {
        public event Action ClosePauseScreen = delegate { };

        void Awake() => ServiceLocator.Global.Register<IUIInputReader>(this);

        public void OnClosePauseScreen(CallbackContext context)
        {
            if (context.performed) ClosePauseScreen.Invoke();
        }

        public void OnCancel(CallbackContext context) { }
        public void OnClick(CallbackContext context) { }
        public void OnMiddleClick(CallbackContext context) { }
        public void OnNavigate(CallbackContext context) { }
        public void OnPoint(CallbackContext context) { }
        public void OnRightClick(CallbackContext context) { }
        public void OnScrollWheel(CallbackContext context) { }
        public void OnSubmit(CallbackContext context) { }
        public void OnTrackedDeviceOrientation(CallbackContext context) { }
        public void OnTrackedDevicePosition(CallbackContext context) { }
    }
}