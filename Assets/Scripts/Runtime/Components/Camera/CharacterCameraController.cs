using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : StatefulEntity, IEnableComponent
    {
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, InLineEditor] CameraData cameraData;
        CameraActiveState activeState;
        CameraDeactiveState deactiveState;

        public bool IsBodyCameraEnabled { get; private set; } = false;
        public CameraRotation CameraRotation { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
            SetupStateMachine();
        }

        void SetupComponents() => CameraRotation = new CameraRotation(cameraData, cinemachine);

        void SetupStateMachine()
        {
            activeState = new CameraActiveState(this);
            deactiveState = new CameraDeactiveState(this);

            At(deactiveState, activeState, () => IsBodyCameraEnabled);
            At(activeState, deactiveState, () => !IsBodyCameraEnabled);

            Set(deactiveState);
        }

        void OnEnable() => PlayerMapInputProvider.Aim.started += OnAimStarted;

        void OnDisable() => PlayerMapInputProvider.Aim.started -= OnAimStarted;

        void OnAimStarted(InputAction.CallbackContext context)
        {
            IsBodyCameraEnabled = !IsBodyCameraEnabled;
            cinemachine.Priority = IsBodyCameraEnabled ? 9 : 1;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            CameraRotation.UpdateRotateBackToInitial();
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);
    }
}
