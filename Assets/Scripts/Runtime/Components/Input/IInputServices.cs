namespace Universal.Runtime.Components.Input
{
    public interface IInputServices
    {
        void EnableActions();
        void SetCursorLocked(bool isSet);
        void ChangeToPlayerMap();
        void ChangeToUIMap();
    }
}