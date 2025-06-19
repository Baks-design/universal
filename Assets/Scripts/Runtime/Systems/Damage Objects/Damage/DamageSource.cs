using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public abstract class DamageSource : MonoBehaviour
    {
        [SerializeField] protected float baseDamage = 10f;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected LayerMask damageableLayers;
        [SerializeField] protected bool canSeverLimbs = true;

        protected DamageResult ApplyDamage(
            IDamageable damageable, Vector3 hitPosition, Vector3 hitDirection)
        {
            var damageInfo = new DamageInfo(
                baseDamage,
                hitPosition,
                hitDirection,
                damageType,
                gameObject
            );
            return damageable.TakeDamage(damageInfo);
        }
    }
}