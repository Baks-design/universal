using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using Universal.Runtime.Utilities.Tools.Updates;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : MonoBehaviour, ILateUpdatable
    {
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, Parent] CharacterController controller;
        [SerializeField] Transform cameraRoot;
        [SerializeField] CameraSettings settings;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        IInputDeviceServices inputServices;
        SwayingHandler swayingHandler;
        RotationHandler rotationHandler;
        AimingHandler aimingHandler;
        BreathingHandler breathingHandler;
        RecoilHandler recoilHandler;

        public RecoilHandler RecoilHandler => recoilHandler;

        void Awake()
        {
            GetServices();
            InitClasses();
        }

        void GetServices()
        {
            ServiceLocator.Global.Get(out movementInput);
            ServiceLocator.Global.Get(out investigateInput);
            ServiceLocator.Global.Get(out combatInput);
            ServiceLocator.Global.Get(out inputServices);
        }
        
        void InitClasses()
        {
            aimingHandler = new AimingHandler(cinemachine, settings);
            rotationHandler = new RotationHandler(
                settings, cameraRoot, aimingHandler, movementInput, investigateInput, combatInput, inputServices);
            swayingHandler = new SwayingHandler(settings, cameraRoot);
            breathingHandler = new BreathingHandler(cinemachine.transform, settings, controller);
            recoilHandler = new RecoilHandler(settings, cinemachine.transform);
        }

        void OnEnable()
        {
            this.AutoRegisterUpdates();

            movementInput.Aim += OnAim;
            investigateInput.Aim += OnAim;
            combatInput.Aim += OnAim;
        }

        void OnDisable()
        {
            movementInput.Aim -= OnAim;
            investigateInput.Aim -= OnAim;
            combatInput.Aim -= OnAim;

            this.AutoUnregisterUpdates();
        }

        void OnAim() => aimingHandler.ChangeFOV(this);

        public void OnLateUpdate()
        {
            rotationHandler.UpdateRotation();
            breathingHandler.UpdateBreathing();
            recoilHandler.UpdateRecoil();
        }

        public void HandleSway(Vector3 inputVector, float rawXInput)
        => swayingHandler.ApplySway(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning)
        => aimingHandler.ChangeRunFOV(this, returning);
    }
}
