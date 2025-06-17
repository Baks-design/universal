namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public interface IState
    {
        void OnEnter() { }
        void FixedUpdate() { }
        void Update() { }
        void LateUpdate() { }
        void OnExit() { }
    }
}