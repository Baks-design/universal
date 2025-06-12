namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public interface IState
    {
        void OnEnter() { }
        void Update() { }
        void FixedUpdate() { }
        void LateUpdate() { }
        void OnExit() { }
    }
}