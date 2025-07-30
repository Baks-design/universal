namespace Universal.Runtime.Components.Input
{
    public interface IInputReaderServices
    {
        void ChangeToMovementMap();
        void ChangeToInvestigateMap();
        void ChangeToCombatMap();
        void ChangeToUIMap();
        void DisableGameInput();
        void SetCursorLocked(bool isSet);
    }
}