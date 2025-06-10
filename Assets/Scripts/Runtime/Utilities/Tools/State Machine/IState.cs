namespace Universal.Runtime.Utilities.Tools.StateMachine
{
    public interface IState
    {
        void Update() { }
        void FixedUpdate() { }
        void LateUpdate() { }
        void OnEnter() { }
        void OnExit() { }
    }
}