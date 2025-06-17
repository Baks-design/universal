using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class CameraDeactiveState : IState
    {
        readonly CharacterCameraController controller;

        public CameraDeactiveState(CharacterCameraController controller)
        => this.controller = controller;

        public void OnEnter() => controller.CameraRotation.ReturnToInitialRotation();
    }
}