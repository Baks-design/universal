namespace Universal.Runtime.Systems.DamageObjects
{
    public readonly struct DamageResult
    {
        public readonly float ActualDamage;
        public readonly DamageSeverity Severity;
        public readonly Limb AffectedLimb;
        public readonly bool WasLimbSevered;

        public DamageResult(
            float damage, DamageSeverity severity, Limb limb, bool severed)
        {
            ActualDamage = damage;
            Severity = severity;
            AffectedLimb = limb;
            WasLimbSevered = severed;
        }
    }
}