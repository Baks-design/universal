using Alchemy.Inspector;
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
        [SerializeField] PerlinNoiseData perlinNoiseData;
        [SerializeField, InlineEditor] CameraData cameraData;
        CameraActiveState activeState;
        CameraDeactiveState deactiveState;

        public bool IsBodyCameraEnabled { get; private set; } = false;
        public CameraRotation CameraRotation { get; private set; }
        public CameraAiming CameraAiming { get; private set; }
        public CameraSwaying CameraSwaying { get; private set; }
        public CameraBreathing CameraBreathing { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
            SetupStateMachine();
        }

        void SetupComponents()
        {
            CameraRotation = new CameraRotation(cameraData, cinemachine);
            CameraAiming = new CameraAiming(cameraData, cinemachine);
            CameraSwaying = new CameraSwaying(cameraData, cinemachine);
            CameraBreathing = new CameraBreathing(perlinNoiseData, cameraData, cinemachine);
        }

        void SetupStateMachine()
        {
            activeState = new CameraActiveState(this);
            deactiveState = new CameraDeactiveState(this);

            At(deactiveState, activeState, () => IsBodyCameraEnabled);
            At(activeState, deactiveState, () => !IsBodyCameraEnabled);

            Set(deactiveState);
        }

        void OnEnable()
        {
            PlayerMapInputProvider.Inspection.started += OnInspectionStarted;
            PlayerMapInputProvider.Aim.started += OnAimingStarted;
            PlayerMapInputProvider.Aim.canceled += OnAimingCanceled;
        }

        void OnDisable()
        {
            PlayerMapInputProvider.Inspection.started -= OnInspectionStarted;
            PlayerMapInputProvider.Aim.started -= OnAimingStarted;
            PlayerMapInputProvider.Aim.canceled -= OnAimingCanceled;
        }

        void OnInspectionStarted(InputAction.CallbackContext context)
        {
            IsBodyCameraEnabled = !IsBodyCameraEnabled;
            cinemachine.Priority = IsBodyCameraEnabled ? 9 : 1;
        }

        void OnAimingStarted(InputAction.CallbackContext context) => CameraAiming.ChangeFOV(this);

        void OnAimingCanceled(InputAction.CallbackContext context) => CameraAiming.ChangeFOV(this);

        protected override void LateUpdate()
        {
            base.LateUpdate();
            CameraRotation.UpdateRotateBackToInitial();
            CameraBreathing.UpdateBreathing();
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        public void HandleSway(Vector3 inputVector, float rawXInput)
        => CameraSwaying.SwayPlayer(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning) => CameraAiming.ChangeRunFOV(returning, this);
    }
}
