using UnityEngine;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class ActiveState : IState
    {
        readonly CharacterCameraController controller;

        public ActiveState(CharacterCameraController controller) => this.controller = controller;

        public void LateUpdate() => controller.CameraRotation.HandleRotation(Time.smoothDeltaTime);
    }
}