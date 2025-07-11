using Alchemy.Inspector;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : MonoBehaviour
    {
        [SerializeField, Parent] CharacterPlayerController controller;
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, InlineEditor] CameraData cameraData;
        CameraRotation cameraRotation;
        CameraAiming cameraAiming;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        Vector2 lookInput;

        public CinemachineCamera Cinemachine { get => cinemachine; set => cinemachine = value; }

        void Awake()
        {
            GetServices();
            InitClasses();
        }

        void GetServices()
        {
            ServiceLocator.Global.Get(out investigateInput);
            ServiceLocator.Global.Get(out combatInput);
        }

        void InitClasses()
        {
            cameraRotation = new CameraRotation(cameraData, cinemachine, controller);
            cameraAiming = new CameraAiming(cameraData, cinemachine);
        }

        void OnEnable()
        {
            investigateInput.Look += OnLook;
            investigateInput.Aim += OnAim;
            combatInput.Look += OnLook;
            combatInput.Aim += OnAim;
        }

        void OnDisable()
        {
            investigateInput.Look -= OnLook;
            investigateInput.Aim -= OnAim;
            combatInput.Look -= OnLook;
            combatInput.Aim -= OnAim;
        }

        void OnLook(Vector2 value) => lookInput = value;

        void OnAim() => cameraAiming.ChangeFOV(this);

        void LateUpdate()
        {
            cameraRotation.ProcessRotation(lookInput);
            cameraRotation.ReturnToInitialRotation(this);
        }
    }
}
