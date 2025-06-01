namespace Universal.Runtime.Systems.Stats
{
    public class AddOperation : IOperationStrategy
    {
        readonly int value;

        public AddOperation(int value) => this.value = value;

        public int Calculate(int value) => value + this.value;
    }
}