using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Utilities.Tools.Updates
{
    public static class UpdateManagerExtensions
    {
        public static void AutoRegisterUpdates(this MonoBehaviour behaviour)
        {
            var updateManager = ServiceLocator.Global.Get<IUpdateManager>();
            if (updateManager == null) return;

            if (behaviour is IUpdatable updatable) updateManager.RegisterUpdatable(updatable);
            if (behaviour is IFixedUpdatable fixedUpdatable) updateManager.RegisterFixedUpdatable(fixedUpdatable);
            if (behaviour is ILateUpdatable lateUpdatable) updateManager.RegisterLateUpdatable(lateUpdatable);
        }

        public static void AutoUnregisterUpdates(this MonoBehaviour behaviour)
        {
            var updateManager = ServiceLocator.Global.Get<IUpdateManager>();
            if (updateManager == null) return;

            if (behaviour is IUpdatable updatable) updateManager.UnregisterUpdatable(updatable);
            if (behaviour is IFixedUpdatable fixedUpdatable) updateManager.UnregisterFixedUpdatable(fixedUpdatable);
            if (behaviour is ILateUpdatable lateUpdatable) updateManager.UnregisterLateUpdatable(lateUpdatable);
        }
    }
}