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
        IPlayerInputReader inputReader;

        public CinemachineCamera Cinemachine { get => cinemachine; set => cinemachine = value; }
        public CameraRotation CameraRotation { get; private set; }
        public CameraAiming CameraAiming { get; private set; }
        public CameraSwaying CameraSwaying { get; private set; }

        void Awake()
        {
            ServiceLocator.Global.Get(out inputReader);
            CameraRotation = new CameraRotation(cameraData, cinemachine, inputReader, this);
            CameraAiming = new CameraAiming(cameraData, cinemachine);
            CameraSwaying = new CameraSwaying(cameraData, cinemachine);
        }
    }
}
