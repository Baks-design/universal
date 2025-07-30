using System;
using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] WeaponSettings weaponData;
        int currentAmmoCount;
        float timeSinceLastShot;
        bool duringReload;

        public WeaponSettings Data => weaponData;
        public int CurrentAmmoCount
        {
            get => currentAmmoCount;
            set => currentAmmoCount = value;
        }
        public float TimeSinceLastShot
        {
            get => timeSinceLastShot;
            set => timeSinceLastShot = value;
        }
        public bool DuringReload
        {
            get => duringReload;
            set => duringReload = value;
        }

        public Action OnWeaponShootPressed = delegate { };
        public Action OnWeaponShootHeld = delegate { };
        public Action OnWeaponShootReleased = delegate { };
        public Action OnWeaponShootSucceed = delegate { };
        public Action OnWeaponReloadPressed = delegate { };
        public Action OnWeaponReloadCompleted = delegate { };

        public virtual void Awake() => InitData();

        public virtual void InitData()
        {
            Data.Init();
            CurrentAmmoCount = Data.ammoCount;
        }

        public virtual void OnShootButtonPressed() => OnWeaponShootPressed();

        public virtual void OnShootButtonHeld() => OnWeaponShootHeld();

        public virtual void OnShootButtonReleased() => OnWeaponShootReleased();

        public virtual void OnReloadButtonPressed() => OnWeaponReloadPressed();
    }
}