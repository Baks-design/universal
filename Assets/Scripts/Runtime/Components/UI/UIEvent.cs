using Universal.Runtime.Utilities.Tools.EventBus;

namespace Universal.Runtime.Components.UI
{
    public struct UIEvent : IEvent
    {
        public bool IsPaused;
    }
}