namespace Universal.Runtime.Systems.DamageObjects
{
    public interface IWeaknessSystem
    {
        void ConfigureLimbWeaknesses(Limb limb);
        float GetDamageMultiplier(LimbType limbType, DamageType damageType);
        bool IsCritical(LimbType limbType, DamageType damageType);
    }
}