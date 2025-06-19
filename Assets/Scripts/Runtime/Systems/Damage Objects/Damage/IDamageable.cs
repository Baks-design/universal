namespace Universal.Runtime.Systems.DamageObjects
{
    public interface IDamageable
    {
        DamageResult TakeDamage(DamageInfo damageInfo);
        DamageResult ApplyLimbDamage(Limb limb, DamageInfo damageInfo);
    }
}