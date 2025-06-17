using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class CameraActiveState : IState
    {
        readonly CharacterCameraController controller;

        public CameraActiveState(CharacterCameraController controller)
        => this.controller = controller;

        public void OnEnter() => controller.CameraRotation.ResetPosition();

        public void LateUpdate() => controller.CameraRotation.HandleRotation();
    }
}