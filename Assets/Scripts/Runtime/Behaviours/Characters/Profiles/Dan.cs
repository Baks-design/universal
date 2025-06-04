using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    public class Dan : Character
    {
        float currentSpeed;
        Coroutine speedBoostRoutine;

        void Start() => currentSpeed = Data.moveSpeed;

        public override void ProcessMovement(float deltaTime)
        => tr.Translate(currentSpeed * deltaTime * Vector3.forward);

        public override void UseAbility()
        {
            Debug.Log("Dan used his speed boost!");
            ApplySpeedBoost(characterData.abilityMultiplier, characterData.abilityDuration);
        }

        void ApplySpeedBoost(float multiplier, float duration)
        {
            if (speedBoostRoutine != null)
                StopCoroutine(speedBoostRoutine);
            speedBoostRoutine = StartCoroutine(SpeedBoost(multiplier, duration));
        }

        IEnumerator SpeedBoost(float multiplier, float duration)
        {
            currentSpeed = Data.moveSpeed * multiplier;
            yield return WaitFor.Seconds(duration);
            currentSpeed = Data.moveSpeed;
        }
    }
}