using Alchemy.Inspector;
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
        [SerializeField, InlineEditor] CameraData cameraData;
        IInvestigateInputReader investigateInput;

        public CinemachineCamera Cinemachine
        {
            get => cinemachine;
            set => cinemachine = value;
        }
        public CameraRotation CameraRotation { get; private set; }
        public CameraAiming CameraAiming { get; private set; }

        void Awake()
        {
            GetServices();
            InitClasses();
        }

        void GetServices() => ServiceLocator.Global.Get(out investigateInput);

        void InitClasses()
        {
            CameraRotation = new CameraRotation(cameraData, cinemachine, investigateInput, this);
            CameraAiming = new CameraAiming(cameraData, cinemachine);
        }
    }
}
