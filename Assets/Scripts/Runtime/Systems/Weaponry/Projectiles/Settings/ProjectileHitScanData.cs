using System;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    [CreateAssetMenu(menuName = "Data/Systems/Weaponry/Projectiles/HitScan")]
    public class ProjectileHitScanData : ProjectileData
    {
        [Serializable]
        public struct ProjectileHitScanSettings
        {
            [SerializeField] float scanDistance;
            public readonly float ScanDistance => scanDistance;
        }

        [SerializeField] ProjectileHitScanSettings specificSettings = new();

        public ProjectileHitScanSettings SpecificSettings => specificSettings;
    }
}