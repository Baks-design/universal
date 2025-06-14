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
        bool isBodyCamEnable = false;
        ActiveState activeState;
        DeactiveState deactiveState;
        private Vector2 lookInput;

        public CameraRotation CameraRotation { get; set; }
        public bool IsActiveCurrentState => stateMachine.CurrentState == activeState;

        protected override void Awake()
        {
            base.Awake();
            SetupStateMachine();
            SetupComponents();
        }

        void SetupStateMachine()
        {
            activeState = new ActiveState(this);
            deactiveState = new DeactiveState(this);

            At(deactiveState, activeState, () => isBodyCamEnable);
            At(activeState, deactiveState, () => !isBodyCamEnable);

            SetState(deactiveState);
        }

        void SetupComponents() => CameraRotation = new CameraRotation(cameraData, cinemachine);

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        void OnEnable() => PlayerMapInputProvider.SetAttackMode.started += SetAttackMode;

        void OnDisable() => PlayerMapInputProvider.SetAttackMode.started -= SetAttackMode;

        void OnDestroy() => PlayerMapInputProvider.SetAttackMode.started -= SetAttackMode;

        void SetAttackMode(InputAction.CallbackContext context)
        {
            isBodyCamEnable = !isBodyCamEnable;
            cinemachine.Priority = isBodyCamEnable ? (PrioritySettings)9 : (PrioritySettings)1;
        }
    }
}
