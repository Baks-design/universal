namespace Universal.Runtime.Systems.Stats
{
    public class MultiplyOperation : IOperationStrategy
    {
        readonly int value;

        public MultiplyOperation(int value) => this.value = value;

        public int Calculate(int value) => value * this.value;
    }
}