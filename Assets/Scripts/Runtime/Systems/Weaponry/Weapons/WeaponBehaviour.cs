using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Camera;

namespace Universal.Runtime.Systems.Weaponry
{
    public class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField, Parent] WeaponController weaponController;
        [SerializeField, Self] Weapon weapon;
        [SerializeField, Parent] CharacterCameraController cameraController;

        protected bool CanShootWeapon
        {
            get
            {
                if (weapon.DuringReload)
                    return false;

                if (weapon.TimeSinceLastShot + weapon.Data.TimeBetweenRounds <
                    Time.time && weapon.CurrentAmmoCount > 0)
                    return true;

                return false;
            }
        }

        void OnEnable()
        {
            weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            weapon.OnWeaponShootReleased += OnWeaponShootReleased;
            weapon.OnWeaponReloadCompleted += OnWeaponReloadCompleted;

            switch (weapon.Data.triggerType)
            {
                case WeaponTriggerType.PullRelease:
                    weapon.OnWeaponShootPressed += OnWeaponShootPressed;
                    break;
                case WeaponTriggerType.Continous:
                    weapon.OnWeaponShootHeld += OnWeaponShootHeld;
                    break;
            }
        }

        void OnDisable()
        {
            weapon.OnWeaponReloadPressed -= OnWeaponReloadPressed;
            weapon.OnWeaponShootReleased -= OnWeaponShootReleased;
            weapon.OnWeaponReloadCompleted -= OnWeaponReloadCompleted;

            switch (weapon.Data.triggerType)
            {
                case WeaponTriggerType.PullRelease:
                    weapon.OnWeaponShootPressed -= OnWeaponShootPressed;
                    break;
                case WeaponTriggerType.Continous:
                    weapon.OnWeaponShootHeld -= OnWeaponShootHeld;
                    break;
            }
        }

        public virtual void OnWeaponReloadPressed()
        {
            weapon.DuringReload = true;
            weapon.CurrentAmmoCount = weapon.Data.ammoCount;
        }

        void OnWeaponShootReleased() { }

        public virtual void OnWeaponReloadCompleted() => weapon.DuringReload = false;

        void OnWeaponShootPressed()
        {
            if (!CanShootWeapon) return;
            OnWeaponShot();
        }

        void OnWeaponShootHeld()
        {
            if (!CanShootWeapon) return;
            OnWeaponShot();
        }

        void OnWeaponShot()
        {
            weapon.OnWeaponShootSucceed();

            weapon.CurrentAmmoCount--;
            weapon.TimeSinceLastShot = Time.time;

            //TODO: Fire from Pool

            weaponController.WeaponRecoil.AddRecoil();
            cameraController.RecoilHandler.AddRecoil();
        }
    }
}
