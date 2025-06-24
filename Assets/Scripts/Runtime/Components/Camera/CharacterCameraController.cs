using Alchemy.Inspector;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : StatefulEntity, IEnableComponent
    {
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, InlineEditor] CameraData cameraData;
        CameraActiveState activeState;
        CameraDeactiveState deactiveState;
        IPlayerInputReader inputReader;

        public bool IsBodyCameraEnabled { get; private set; } = false;
        public CameraRotation CameraRotation { get; private set; }
        public CameraAiming CameraAiming { get; private set; }
        public CameraSwaying CameraSwaying { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
            SetupStateMachine();
        }

        void SetupComponents()
        {
            ServiceLocator.Global.Get(out inputReader);

            CameraRotation = new CameraRotation(cameraData, cinemachine, inputReader);
            CameraAiming = new CameraAiming(cameraData, cinemachine);
            CameraSwaying = new CameraSwaying(cameraData, cinemachine);
        }

        void SetupStateMachine()
        {
            activeState = new CameraActiveState(this);
            deactiveState = new CameraDeactiveState(this);

            At(deactiveState, activeState, () => IsBodyCameraEnabled);
            At(activeState, deactiveState, () => !IsBodyCameraEnabled);

            Set(deactiveState);
        }

        void OnEnable() => RegisterInputs();

        void OnDisable() => UnregisterInputs();

        void RegisterInputs()
        {
            inputReader.Inspection += OnInspection;
            inputReader.Aim += OnAiming;
        }

        void UnregisterInputs()
        {
            inputReader.Inspection -= OnInspection;
            inputReader.Aim -= OnAiming;
        }

        void OnInspection()
        {
            IsBodyCameraEnabled = !IsBodyCameraEnabled;
            cinemachine.Priority = IsBodyCameraEnabled ? 9 : 1;
        }

        void OnAiming()
        {
            if (!IsBodyCameraEnabled) return;
            CameraAiming.ChangeFOV(this);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            CameraRotation.UpdateRotateBackToInitial();
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        public void HandleSway(Vector3 inputVector, float rawXInput)
        => CameraSwaying.SwayPlayer(inputVector, rawXInput);
    }
}
