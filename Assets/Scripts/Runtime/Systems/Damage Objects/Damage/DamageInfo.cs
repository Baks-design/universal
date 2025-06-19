using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector3 HitPosition;
        public readonly Vector3 HitDirection;
        public readonly DamageType Type;
        public readonly GameObject DamageSource;

        public DamageInfo(
            float amount, Vector3 position, Vector3 direction,
            DamageType type, GameObject source = null)
        {
            Amount = amount;
            HitPosition = position;
            HitDirection = direction;
            Type = type;
            DamageSource = source;
        }
    }
}