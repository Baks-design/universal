using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Camera
{
    public class CharacterCameraController : MonoBehaviour
    {
        [SerializeField, Child] CinemachineCamera cinemachine;
        [SerializeField, Parent] CharacterController controller;
        [SerializeField] Transform cameraRoot;
        [SerializeField] CameraSettings settings;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
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
        }

        void InitClasses()
        {
            aimingHandler = new AimingHandler(cinemachine, settings);
            rotationHandler = new RotationHandler(
                settings, cameraRoot, aimingHandler, movementInput, investigateInput, combatInput);
            swayingHandler = new SwayingHandler(settings, cameraRoot);
            breathingHandler = new BreathingHandler(cinemachine.transform, settings, controller);
            recoilHandler = new RecoilHandler(settings, cinemachine.transform);
        }

        void OnEnable()
        {
            movementInput.Aim += OnAim;
            investigateInput.Aim += OnAim;
            combatInput.Aim += OnAim;
        }

        void OnDisable()
        {
            movementInput.Aim -= OnAim;
            investigateInput.Aim -= OnAim;
            combatInput.Aim -= OnAim;
        }

        void OnAim() => aimingHandler.ChangeFOV(this);

        void LateUpdate()
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
