using System;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class WeaponDamageDealer : MonoBehaviour
    {
        [SerializeField] DamageType damageType;
        [SerializeField] float baseDamage = 10f;
        [SerializeField, Range(0f, 1f)] float criticalChance = 0.15f;
        [SerializeField] float criticalMultiplier = 2f;
        [SerializeField, Range(0f, 0.5f)] float damageVariance = 0.1f;

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out DamageCurrencySystem damageable)) return;

            var packet = new DamagePacket
            {
                attacker = gameObject,
                damage = baseDamage,
                type = damageType,
                criticalChance = criticalChance,
                criticalMultiplier = criticalMultiplier,
                damageVariance = damageVariance
            };
            damageable.TakeDamage(packet);
        }
    }
}