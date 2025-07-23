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
        [SerializeField] Transform cam;
        [SerializeField] CameraSettings settings;
        Vector2 lookInput;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        CameraSwaying cameraSwaying;
        CameraRotation cameraRotation;
        CameraAiming cameraAiming;

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
            cameraAiming = new CameraAiming(this, settings, cinemachine);
            cameraRotation = new CameraRotation(settings, cam, cameraAiming);
            cameraSwaying = new CameraSwaying(settings, cam);
        }

        void OnEnable()
        {
            movementInput.Look += OnLook;
            movementInput.Aim += OnAim;

            investigateInput.Look += OnLook;
            investigateInput.Aim += OnAim;

            combatInput.Look += OnLook;
            combatInput.Aim += OnAim;
        }

        void OnDisable()
        {
            movementInput.Look -= OnLook;
            movementInput.Aim -= OnAim;

            investigateInput.Look -= OnLook;
            investigateInput.Aim -= OnAim;

            combatInput.Look -= OnLook;
            combatInput.Aim -= OnAim;
        }

        void OnLook(Vector2 value) => lookInput = value;

        void OnAim() => cameraAiming.ToggleZoom();

        void LateUpdate() => cameraRotation.UpdateRotation(lookInput);

        public void HandleSway(Vector3 inputVector, float rawXInput)
        => cameraSwaying.ApplySway(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning) => cameraAiming.SetZoom(returning);
    }
}
