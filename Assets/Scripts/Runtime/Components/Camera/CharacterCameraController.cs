using System;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : MonoBehaviour, IEnableComponent
    {
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField] Transform yawTransform;
        [SerializeField] Transform pitchTransform;
        [SerializeField, InLineEditor] CameraData cameraData;
        StateMachine stateMachine;
        bool isCameraActive = false;

        public CameraRotation CameraRotation { get; set; }

        void Awake()
        {
            SetupStateMachine();
            CameraRotation = new CameraRotation(cameraData, yawTransform, pitchTransform);
        }

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            var activeState = new ActiveState(this);
            var deactiveState = new DeactiveState(this);

            At(deactiveState, activeState, () => isCameraActive);
            At(activeState, deactiveState, () => !isCameraActive);

            stateMachine.SetState(deactiveState);
        }

        void At(IState from, IState to, Func<bool> condition)
        => stateMachine.AddTransition(from, to, condition);

        void LateUpdate() => stateMachine.LateUpdate();

        void Update() => Debug.Log($"Current State: {stateMachine.CurrentState}");

        void OnEnable() => PlayerMapInputProvider.SetAttackMode.started += SetAttackMode;

        void OnDisable() => PlayerMapInputProvider.SetAttackMode.started -= SetAttackMode;

        void OnDestroy() => PlayerMapInputProvider.SetAttackMode.started -= SetAttackMode;

        void SetAttackMode(InputAction.CallbackContext context)
        {
            isCameraActive = !isCameraActive;
            if (isCameraActive)
                cinemachine.Priority = 9;
            else
                cinemachine.Priority = 1;
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);
    }
}
