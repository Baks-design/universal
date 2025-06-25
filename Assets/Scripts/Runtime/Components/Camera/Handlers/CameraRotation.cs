using Unity.Cinemachine;
using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.Camera
{
    public class CameraRotation
    {
        readonly CameraMovement movement;
        readonly CameraRecentering recentering;

        public bool IsRecentering => recentering.IsRecentering;

        public CameraRotation(
            CameraData data,
            CinemachineCamera target,
            IPlayerInputReader inputReader,
            MonoBehaviour monoBehaviour)
        {
            var initialRotation = target.transform.localRotation;

            movement = new CameraMovement(data, target, inputReader, initialRotation);
            recentering = new CameraRecentering(data, movement, initialRotation, monoBehaviour);
        }

        public void ProcessRotation()
        {
            if (recentering.IsRecentering) return;
            movement.ProcessRotation();
        }

        public void ReturnToInitialRotation() => recentering.ReturnToInitialRotation();
    }
}