using System;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class Limb
    {
        public LimbType Type { get; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public Transform LimbTransform { get; }
        public bool IsSevered { get; set; }
        public LimbWeaknessConfig WeaknessConfig { get; set; }

        public event Action<Limb, float, DamageSeverity> OnDamageTaken = delegate { };
        public event Action<Limb> OnSevered = delegate { };

        public Limb(LimbType type, float maxHealth, Transform limbTransform)
        {
            Type = type;
            MaxHealth = maxHealth;
            Health = maxHealth;
            LimbTransform = limbTransform;
        }

        public void ConfigureWeaknesses(LimbWeaknessConfig config) => WeaknessConfig = config;

        public DamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (IsSevered) return new DamageResult(0f, DamageSeverity.Normal, this, false);

            WeaknessProfile weakness = null;
            if (WeaknessConfig != null)
                weakness = WeaknessConfig.GetWeaknessProfile(damageInfo.Type);
            var multiplier = weakness?.Multiplier ?? 1f;
            var severity = weakness?.Severity ?? DamageSeverity.Normal;

            var actualDamage = damageInfo.Amount * multiplier;
            Health = Mathf.Max(0, Health - actualDamage);

            var wasSevered = false;
            if (Health <= 0f && !IsSevered)
            {
                Sever();
                wasSevered = true;
            }

            OnDamageTaken.Invoke(this, actualDamage, severity);
            return new DamageResult(actualDamage, severity, this, wasSevered);
        }

        void Sever()
        {
            IsSevered = true;
            OnSevered.Invoke(this);
        }

        public void Heal(float amount) => Health = Mathf.Min(MaxHealth, Health + amount);
    }
}