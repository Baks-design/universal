using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterInvestigationState : IState
    {
        readonly CharacterPlayerController controller;

        public CharacterInvestigationState(CharacterPlayerController controller) => this.controller = controller;
      
        public void OnEnter() => controller.InputServices.ChangeToInvestigateMap();

        public void Update() => controller.CharacterDetectController.DetectBodies();

        public void LateUpdate() => controller.CameraController.CameraRotation.ProcessRotation();
    }
}