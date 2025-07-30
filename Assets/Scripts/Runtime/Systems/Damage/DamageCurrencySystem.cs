using System;
using System.Collections.Generic;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;
using static Freya.Random;

namespace Universal.Runtime.Systems.DamageObjects
{
    public enum DamageType { Physical, Fire, Ice, Lightning, Poison, Arcane }

    public class DamageCurrencySystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] float baseHealth = 100f;
        [SerializeField] DamageModifier[] weaknesses;
        [SerializeField] DamageModifier[] resistances;

        [Header("Currency Settings")]
        [SerializeField] float currencyConversionRate = 1f;
        [SerializeField] bool scaleCurrencyWithModifiers = true;
        [SerializeField] AnimationCurve currencyScalingByHealth = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        float currentHealth;
        Dictionary<DamageType, DamageModifier> weaknessMap;
        Dictionary<DamageType, DamageModifier> resistanceMap;

        public event Action<GameObject, float> OnDamageTaken = delegate { };
        public event Action<GameObject, float> OnCurrencyEarned = delegate { };
        public event Action OnDeath = delegate { };

        void Awake()
        {
            currentHealth = baseHealth;
            InitializeModifierMaps();
        }

        void InitializeModifierMaps()
        {
            weaknessMap = new Dictionary<DamageType, DamageModifier>();
            for (var i = 0; i < weaknesses.Length; i++)
            {
                var mod = weaknesses[i];
                weaknessMap[mod.type] = mod;
            }
            resistanceMap = new Dictionary<DamageType, DamageModifier>();
            for (var i1 = 0; i1 < resistances.Length; i1++)
            {
                var mod = resistances[i1];
                resistanceMap[mod.type] = mod;
            }
        }

        public void TakeDamage(DamagePacket damagePacket)
        {
            if (currentHealth <= 0f) return;

            // Calculate final damage
            var finalDamage = CalculateFinalDamage(damagePacket);
            currentHealth -= finalDamage;

            // Calculate and award currency
            var currencyEarned = CalculateCurrencyEarned(finalDamage, damagePacket);
            AwardCurrency(damagePacket.attacker, currencyEarned);

            // Invoke events
            OnDamageTaken.Invoke(gameObject, finalDamage);
            Logging.Log($"{name} took {finalDamage} {damagePacket.type} damage. " +
                     $"{damagePacket.attacker.name} earned {currencyEarned} currency.");

            if (currentHealth <= 0f) Die();
        }

        void Die()
        {
            OnDeath.Invoke();
            Logging.Log($"{name} has been defeated!");
            gameObject.SetActive(false);
        }

        float CalculateFinalDamage(DamagePacket packet)
        {
            var variedDamage = packet.damage * (1f + Range(-packet.damageVariance, packet.damageVariance));

            var damage = packet.isGuaranteedCritical || Value <= packet.criticalChance
                ? variedDamage * packet.criticalMultiplier
                : variedDamage;

            return CalculateModifiedDamage(damage, packet.type);
        }

        float CalculateModifiedDamage(float damage, DamageType type)
        {
            var modifier = 1f;

            if (weaknessMap.TryGetValue(type, out DamageModifier weakness))
                modifier *= 1f + weakness.multiplier;

            if (resistanceMap.TryGetValue(type, out DamageModifier resistance))
                modifier *= 1f - resistance.multiplier;

            return damage * Clamp(modifier, 0.1f, 3f);
        }

        float CalculateCurrencyEarned(float finalDamage, DamagePacket packet)
        {
            var baseAmount = scaleCurrencyWithModifiers ? finalDamage : packet.damage;
            var scaledAmount = baseAmount * currencyConversionRate;

            var healthScale = currencyScalingByHealth.Evaluate(currentHealth / baseHealth);
            scaledAmount *= healthScale;

            if (weaknessMap.TryGetValue(packet.type, out var weakness) && weakness.currencyBonus > 0f)
                scaledAmount *= 1f + weakness.currencyBonus;

            return scaledAmount;
        }

        void AwardCurrency(GameObject attacker, float amount)
        {
            if (attacker == null || amount <= 0f) return;

            OnCurrencyEarned.Invoke(attacker, amount);
        }

        public void ResetHealth()
        {
            currentHealth = baseHealth;
            gameObject.SetActive(true);
        }
    }
}