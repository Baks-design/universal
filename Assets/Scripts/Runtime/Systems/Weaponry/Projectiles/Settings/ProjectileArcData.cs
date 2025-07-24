using System;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    [CreateAssetMenu(menuName = "Data/System/Weaponry/Projectiles/Arc")]
    public class ProjectileArcData : ProjectileData
    {
        [Serializable]
        public struct ProjectileArcSettings
        {
            [SerializeField] float angle;

            public readonly float Angle => angle;
        }

        [SerializeField] ProjectileArcSettings specificSettings = new();

        public ProjectileArcSettings SpecificSettings => specificSettings;
    }
}