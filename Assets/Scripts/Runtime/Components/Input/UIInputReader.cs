using System;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using static UnityEngine.InputSystem.InputAction;
using static Universal.Runtime.Components.Input.GameInputs;

namespace Universal.Runtime.Components.Input
{
    public class UIInputReader : MonoBehaviour, IUIActions, IUIInputReader
    {
        public event Action Unpause = delegate { };

        void Awake() => ServiceLocator.Global.Register<IUIInputReader>(this);

        public void OnUnpause(CallbackContext context)
        {
            if (context.started) Unpause.Invoke();
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