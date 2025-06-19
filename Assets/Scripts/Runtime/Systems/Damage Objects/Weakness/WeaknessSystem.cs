using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class WeaknessSystem : MonoBehaviour, IWeaknessSystem
    {
        [SerializeField] LimbWeaknessConfig[] limbWeaknessConfigs;
        Dictionary<LimbType, LimbWeaknessConfig> weaknessMap;

        void Awake()
        {
            weaknessMap = new Dictionary<LimbType, LimbWeaknessConfig>();
            for (var i = 0; i < limbWeaknessConfigs.Length; i++)
            {
                var config = limbWeaknessConfigs[i];
                weaknessMap[config.LimbType] = config;
            }
        }

        public void ConfigureLimbWeaknesses(Limb limb)
        {
            if (weaknessMap.TryGetValue(limb.Type, out var config))
                limb.ConfigureWeaknesses(config);
        }

        public float GetDamageMultiplier(LimbType limbType, DamageType damageType)
        {
            if (weaknessMap.TryGetValue(limbType, out var config))
            {
                var weakness = config.GetWeaknessProfile(damageType);
                return weakness?.Multiplier ?? 1f;
            }
            return 1f;
        }

        public bool IsCritical(LimbType limbType, DamageType damageType)
        {
            if (weaknessMap.TryGetValue(limbType, out var config))
            {
                var weakness = config.GetWeaknessProfile(damageType);
                return weakness?.Severity is DamageSeverity.Critical;
            }
            return false;
        }
    }
}