using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    [CreateAssetMenu(menuName = "Data/Damage/Limb Weakness Config")]
    public class LimbWeaknessConfig : ScriptableObject
    {
        public LimbType LimbType;
        public List<WeaknessProfile> Weaknesses = new();

        public WeaknessProfile GetWeaknessProfile(DamageType damageType)
        {
            for (var i = 0; i < Weaknesses.Count; i++)
            {
                var weakness = Weaknesses[i];
                if (weakness.DamageType == damageType)
                    return weakness;
            }
            return null;
        }
    }
}