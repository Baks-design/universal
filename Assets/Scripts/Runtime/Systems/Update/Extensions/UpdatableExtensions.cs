using Universal.Runtime.Systems.Update.Interface;

namespace Universal.Runtime.Systems.Update.Extensions
{
    public static class UpdatableExtensions
    {
        /// <summary>
        /// Shortcut for <c>UpdateManager.Instance.Register(<paramref name="updatable"/>)</c>.
        /// </summary>
        /// <seealso cref="UpdateManager.Register"/>
        public static void RegisterInManager(this IManagedObject updatable)
        => UpdateManager.Instance.Register(updatable);

        /// <summary>
        /// Shortcut for <c>UpdateManager.Instance.Unregister(<paramref name="updatable"/>)</c>.
        /// </summary>
        /// <seealso cref="UpdateManager.Unregister"/>
        public static void UnregisterInManager(this IManagedObject updatable)
        => UpdateManager.Instance.Unregister(updatable);
    }
}
