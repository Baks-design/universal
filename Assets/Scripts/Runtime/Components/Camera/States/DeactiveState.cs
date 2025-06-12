using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Components.Camera
{
    public class DeactiveState : IState
    {
        readonly CharacterCameraController controller;

        public DeactiveState(CharacterCameraController controller) => this.controller = controller;
    }
}