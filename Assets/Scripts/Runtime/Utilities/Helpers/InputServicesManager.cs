using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Utilities.Helpers
{
    public class InputServicesManager : PersistentSingleton<InputServicesManager>
    {
        protected override void Awake()
        {
            base.Awake();
            PlayerMapInputProvider.SetCursor(true);
            PlayerMapInputProvider.InitializeActions();
        }
    }
}
