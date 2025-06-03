using System.Collections;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    [CreateAssetMenu(menuName = "Data/Abilities/Speed Boost")]
    public class SpeedBoostAbilitySO : AbilityDataSO
    {
        [SerializeField] float speedMultiplier = 2f;
        [SerializeField] float duration = 3f;

        public override void UseAbility(Character character)
        {
            if (character is Dan dan)
                dan.StartCoroutine(ApplySpeedBoost(dan));
        }

        private IEnumerator ApplySpeedBoost(Dan dan)
        {
            var originalSpeed = dan.Data.moveSpeed;
            dan.Data.moveSpeed *= speedMultiplier;
            yield return WaitFor.Seconds(duration);
            dan.Data.moveSpeed = originalSpeed;
        }
    }
}