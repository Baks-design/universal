using UnityEngine;
using KBCore.Refs;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class Damageable : MonoBehaviour, IDamageable 
    {
        [SerializeField, Child] HumanoidLimbSystem limbSystem;
        [SerializeField, Child] WeaknessSystem weaknessSystem;
        //[SerializeField, Child] VisualDamageSystem visualDamageSystem; 

        void Awake()
        {
            // Configure weaknesses for all limbs
            var array = limbSystem.GetLimbs();
            for (var i = 0; i < array.Length; i++)
                weaknessSystem.ConfigureLimbWeaknesses(array[i]);
        }

        public DamageResult TakeDamage(DamageInfo damageInfo)
        {
            var closestLimb = FindClosestLimb(damageInfo.HitPosition);
            return closestLimb != null ?
                ApplyLimbDamage(closestLimb, damageInfo) :
                new DamageResult(0f, DamageSeverity.Normal, null, false);
        }

        public DamageResult ApplyLimbDamage(Limb limb, DamageInfo damageInfo)
        {
            var result = limb.TakeDamage(damageInfo);
            //visualDamageSystem.ApplyVisualDamage(limb, limb.Health / limb.MaxHealth, result.Severity);
            return result;
        }

        Limb FindClosestLimb(Vector3 position)
        {
            Limb closest = null;
            var closestDistance = float.MaxValue;

            var array = limbSystem.GetLimbs();
            for (var i = 0; i < array.Length; i++)
            {
                var limb = array[i];
                if (limb.IsSevered) continue;

                var distance = Vector3.Distance(position, limb.LimbTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = limb;
                }
            }

            return closest;
        }
    }
}