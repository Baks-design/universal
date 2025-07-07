namespace Universal.Runtime.Systems.TargetingSelection
{
    public interface IDamageablePart
    {
        float HitMultiplier { get; }

        void TakeDamage(float amount);
    }
}