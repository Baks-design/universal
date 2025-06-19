namespace Universal.Runtime.Systems.InventoryManagement
{
    public class ViewModel
    {
        public readonly int Capacity;
        public readonly BindableProperty<string> Coins;

        public ViewModel(InventoryModel model, int capacity)
        {
            Capacity = capacity;
            Coins = BindableProperty<string>.Bind(() => model.Coins.ToString());
        }
    }
}