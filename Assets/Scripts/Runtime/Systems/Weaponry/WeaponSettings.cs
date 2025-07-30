using UnityEngine;

namespace Universal.Runtime.Systems.Weaponry
{
    public enum WeaponTriggerType
    {
        PullRelease,
        Continous
    }

    [CreateAssetMenu(menuName = "Data/Systems/Weaponry/Settings")]
    public class WeaponSettings : ScriptableObject
    {
        [Header("Ammo Settings")]
        public WeaponTriggerType triggerType;
        [Tooltip("-1 = infinite")] public int ammoCount = 0;
        public int roundsPerSecond = 0;
        public float reloadDuration = 0f;
        float timeBetweenRounds;

        [Header("Recoil Settings")]
        public float verticalRecoilMultiplier = 1f;
        public float horizontalRecoilMultiplier = 1f;
        public float recoilDuration = 0.1f;
        public float recoveryDuration = 0.5f;
        public float snappiness = 10f;
        public float returnSpeed = 10f;

        public float TimeBetweenRounds => timeBetweenRounds;

        public void Init() => timeBetweenRounds = 1f / roundsPerSecond;
    }
}
