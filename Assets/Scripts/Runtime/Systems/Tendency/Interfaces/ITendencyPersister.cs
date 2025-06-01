namespace Universal.Runtime.Systems.Tendency.Interfaces
{
    public interface ITendencyPersister
    {
        void SaveTendencies(float[] tendencies);
        float[] LoadTendencies();
    }
}