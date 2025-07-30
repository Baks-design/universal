using System;

namespace Universal.Runtime.Utilities.Tools.Updates
{
    public interface IUpdateManager
    {
        void RegisterUpdatable(IUpdatable updatable, UpdatePriority priority = UpdatePriority.Normal);
        void RegisterFixedUpdatable(IFixedUpdatable fixedUpdatable);
        void RegisterLateUpdatable(ILateUpdatable lateUpdatable);
        void UnregisterUpdatable(IUpdatable updatable);
        void UnregisterFixedUpdatable(IFixedUpdatable fixedUpdatable);
        void UnregisterLateUpdatable(ILateUpdatable lateUpdatable);
        void AddToUpdateGroup(IUpdatable updatable, string groupName);
        void SetGroupActive(string groupName, bool active);
        void ChangePriority(IUpdatable updatable, UpdatePriority newPriority);
        void QueueAsyncUpdate(Action updateAction);
        void DisplayProfilingResults();
    }
}