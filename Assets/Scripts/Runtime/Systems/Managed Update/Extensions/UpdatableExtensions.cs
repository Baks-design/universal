namespace Universal.Runtime.Systems.ManagedUpdate
{
    public static class UpdatableExtensions
    {
        public static void RegisterInManager(this IManagedObject updatable)
        {
            if (ApplicationUtils.IsQuitting || updatable == null)
                return;

            try
            {
                UpdateManager.Instance.Register(updatable);
            }
            catch
            {
                // Silently handle any instance creation issues
            }
        }

        public static void UnregisterInManager(this IManagedObject updatable)
        {
            if (ApplicationUtils.IsQuitting || updatable == null)
                return;

            try
            {
                UpdateManager.Instance.Unregister(updatable);
            }
            catch
            {
                // Silently handle any instance creation issues
            }
        }
    }
}
