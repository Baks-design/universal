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
        [SerializeField, Parent] InterfaceRef<IGridMover> mover;
        [SerializeField] Transform height;
        [SerializeField, InlineEditor] CameraData data;
        CameraRotation cameraRotation;
        CameraHeadbob cameraHeadbob;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        Vector2 lookInput;

        public CameraAiming CameraAiming { get; private set; }

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
            CameraAiming = new CameraAiming(data, cinemachine);
            cameraRotation = new CameraRotation(data, cinemachine, controller, this);
            cameraHeadbob = new CameraHeadbob(height, data);
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

        void OnAim() => CameraAiming.ToggleZoom(this);

        void LateUpdate()
        {
            cameraHeadbob.ProcessHeadbob(mover.Value);
            cameraRotation.ProcessRotation(lookInput);
            cameraRotation.ReturnToInitialRotation(this);
        }
    }
}
