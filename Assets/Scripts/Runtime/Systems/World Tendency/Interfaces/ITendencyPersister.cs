namespace Universal.Runtime.Systems.WorldTendency
{
    public interface ITendencyPersister
    {
        void SaveTendencies(float[] tendencies);
        float[] LoadTendencies();
    }
}