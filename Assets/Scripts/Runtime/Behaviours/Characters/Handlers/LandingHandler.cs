using System.Collections;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class LandingHandler
    {
        readonly MonoBehaviour monoBehaviour;
        readonly Transform cameraTransform;
        readonly MovementSettings movementSettings;
        readonly CharacterCollisionController collisionController;
        Coroutine activeLandingCoroutine;
        float inAirTime;
        const float Epsilon = 0.001f;

        public LandingHandler(
            MonoBehaviour monoBehaviour,
            Transform cameraTransform,
            MovementSettings movementSettings,
            CharacterCollisionController collisionController)
        {
            this.monoBehaviour = monoBehaviour;
            this.cameraTransform = cameraTransform;
            this.movementSettings = movementSettings;
            this.collisionController = collisionController;
        }

        public void UpdateInAirTime() => inAirTime = collisionController.IsGrounded ? 0f : inAirTime + Time.deltaTime;

        public void HandleLanding()
        {
            if (!ShouldPlayLandingEffect()) return;

            PlayLandingEffect();
        }

        bool ShouldPlayLandingEffect()
        => collisionController.IsGrounded &&
            !collisionController.JustLanded &&
            inAirTime > Epsilon; // Small threshold to ignore tiny falls

        void PlayLandingEffect()
        {
            if (activeLandingCoroutine != null)
                monoBehaviour.StopCoroutine(activeLandingCoroutine);
            
            activeLandingCoroutine = monoBehaviour.StartCoroutine(LandingAnimationRoutine());
        }

        IEnumerator LandingAnimationRoutine()
        {
            var elapsedTime = 0f;
            var duration = movementSettings.landDuration;
            var inverseDuration = 1f / duration;

            // Determine land intensity based on air time
            var landIntensity = inAirTime > movementSettings.landTimer
                ? movementSettings.highLandAmount
                : movementSettings.lowLandAmount;

            var initialLocalPosition = cameraTransform.localPosition;
            var initialYPosition = initialLocalPosition.y;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime * inverseDuration;
                var curveValue = movementSettings.landCurve.Evaluate(progress);

                // Apply the landing effect
                var newPosition = initialLocalPosition;
                newPosition.y = initialYPosition + (curveValue * landIntensity);
                cameraTransform.localPosition = newPosition;

                yield return null;
            }

            // Ensure we return to exactly the initial position
            cameraTransform.localPosition = initialLocalPosition;
            activeLandingCoroutine = null;
        }
    }
}