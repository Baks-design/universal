using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.Weaponry
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] WeaponSettings settings;
        [SerializeField, Child] Weapon[] weapons;
        ICombatInputReader combatInput;
        WeaponRecoil weaponRecoil;

        public WeaponRecoil WeaponRecoil => weaponRecoil;

        void Awake()
        {
            ServiceLocator.Global.Get(out combatInput);
            weaponRecoil = new WeaponRecoil(transform, settings);
        }

        void OnEnable()
        {
            combatInput.Attack += OnWeaponShoot;
            combatInput.AttackHold += OnWeaponHold;
            combatInput.Reload += OnWeaponReload;
        }

        void OnDisable()
        {
            combatInput.Attack -= OnWeaponShoot;
            combatInput.AttackHold -= OnWeaponHold;
            combatInput.Reload -= OnWeaponReload;
        }

        void OnWeaponShoot(bool value)
        {
            if (value)
                for (var i = 0; i < weapons.Length; i++)
                    weapons[i].OnShootButtonPressed();
            else
                for (var i = 0; i < weapons.Length; i++)
                    weapons[i].OnShootButtonReleased();
        }

        void OnWeaponHold(bool value)
        {
            if (!value) return;

            for (var i = 0; i < weapons.Length; i++)
                weapons[i].OnShootButtonHeld();
        }

        void OnWeaponReload()
        {
            for (var i = 0; i < weapons.Length; i++)
                weapons[i].OnReloadButtonPressed();
        }

        void Update() => weaponRecoil.UpdateMotion();
    }
}