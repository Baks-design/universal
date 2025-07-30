using System;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    [Serializable]
    public struct DamageModifier
    {
        public DamageType type;
        [Range(-1f, 2f)] public float multiplier;
        public float currencyBonus;
    }
}