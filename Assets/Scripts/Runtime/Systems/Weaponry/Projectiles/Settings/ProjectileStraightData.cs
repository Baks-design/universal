using System;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    [CreateAssetMenu(menuName = "Data/System/Weapons/Projectiles/Straight")]
    public class ProjectileStraightData : ProjectileData
    {
        [Serializable]
        public struct ProjectileStraightSettings 
        {
            [SerializeField] float speed;

            public readonly float Speed => speed;
        }

        [SerializeField] protected ProjectileStraightSettings specificSettings = new();

        public ProjectileStraightSettings SpecificSettings => specificSettings;
    }
}