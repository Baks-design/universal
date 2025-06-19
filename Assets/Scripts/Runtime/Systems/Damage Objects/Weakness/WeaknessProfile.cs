using System;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    [Serializable]
    public class WeaknessProfile
    {
        public DamageType DamageType;
        [Range(0.1f, 3f)] public float Multiplier = 1f;
        public DamageSeverity Severity = DamageSeverity.Normal;
    }
}