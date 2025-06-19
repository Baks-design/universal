using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public interface IVisualDamageSystem
    {
        void ApplyVisualDamage(Limb limb, float healthPercentage, DamageSeverity severity);
        void Initialize(GameObject characterModel);
    }
}