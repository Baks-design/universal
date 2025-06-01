namespace Universal.Runtime.Systems.Stats
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}