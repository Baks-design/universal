using System;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    [CreateAssetMenu( menuName = "Data/Systems/Weaponry/Projectiles/Missile")]
    public class ProjectileMissileData : ProjectileData
    {
        [Serializable]
        public struct ProjectileMissileSettings 
        {
            [SerializeField] float followDuration;

            public readonly float FollowDuration => followDuration;
        }

        [SerializeField] ProjectileMissileSettings specificSettings = new();

        public ProjectileMissileSettings SpecificSettings => specificSettings;
    }
}