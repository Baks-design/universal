using System;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    [Serializable]
    public struct DamagePacket
    {
        public GameObject attacker;
        public float damage;
        public DamageType type;
        public float criticalChance;
        public float criticalMultiplier;
        public float damageVariance;
        public bool isGuaranteedCritical;
    }
}