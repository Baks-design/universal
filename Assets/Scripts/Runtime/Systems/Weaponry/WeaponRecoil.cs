using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Random;

namespace Universal.Runtime.Systems.Weaponry
{
    public class WeaponRecoil
    {
        readonly Transform transform;
        readonly WeaponSettings settings;
        readonly RecoilPattern recoilPattern;
        Quaternion currentRotation;
        Vector3 targetRotation;

        public WeaponRecoil(Transform transform, WeaponSettings settings)
        {
            this.transform = transform;
            this.settings = settings;

            recoilPattern = new RecoilPattern();
        }

        public void UpdateMotion()
        {
            targetRotation = Helpers.ExpDecay(
                targetRotation, Vector3.zero, settings.returnSpeed * Time.deltaTime);

            transform.localRotation = Helpers.ExpDecayRotation(
                Quaternion.Euler(0f, currentRotation.y, 0f),
                Quaternion.Euler(0f, targetRotation.y, 0f),
                settings.snappiness * Time.deltaTime);
        }

        public void AddRecoil()
        {
            var patternPoint = recoilPattern.GetCurrentPoint();

            var recoilRotation = new Vector3(
                patternPoint.y * settings.verticalRecoilMultiplier,
                patternPoint.x * settings.horizontalRecoilMultiplier,
                Range(-1f, 1f)
            );

            targetRotation += recoilRotation;

            recoilPattern.AdvancePattern();
        }
    }
}
