using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterInvestigationState : IState
    {
        readonly PlayerController controller;

        public void OnEnter() => controller.inputServices.ChangeToInvestigateMap();

        public CharacterInvestigationState(PlayerController controller) => this.controller = controller;

        public void LateUpdate() => controller.CameraController.CameraRotation.ProcessRotation();
    }
}