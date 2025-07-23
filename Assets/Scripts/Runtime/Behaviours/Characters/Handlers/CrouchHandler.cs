using System.Collections;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CrouchHandler
    {
        readonly MonoBehaviour monoBehaviour;
        readonly CharacterController characterController;
        readonly Transform cameraTransform;
        readonly MovementSettings movementSettings;
        readonly CharacterCollisionController collisionController;
        readonly Vector3 initialCenter;
        readonly Vector3 crouchCenter;
        readonly float initialHeight;
        readonly float crouchHeight;
        readonly float initialCameraHeight;
        readonly float crouchCameraHeight;
        readonly float crouchStandHeightDifference;
        Coroutine activeCrouchCoroutine;
        bool isDuringCrouchAnimation;

        public bool DuringCrouchAnimation => isDuringCrouchAnimation;
        public bool IsCrouching { get; private set; }

        public CrouchHandler(
            MonoBehaviour monoBehaviour,
            CharacterController characterController,
            Transform cameraTransform,
            MovementSettings movementSettings,
            CharacterCollisionController collisionController)
        {
            this.monoBehaviour = monoBehaviour;
            this.characterController = characterController;
            this.cameraTransform = cameraTransform;
            this.movementSettings = movementSettings;
            this.collisionController = collisionController;

            // Initialize controller dimensions
            initialHeight = characterController.height;
            initialCenter = new Vector3(0f, initialHeight / 2f + characterController.skinWidth, 0f);
            characterController.center = initialCenter;

            // Calculate crouch dimensions
            crouchHeight = initialHeight * movementSettings.crouchPercent;
            crouchCenter = new Vector3(0f, crouchHeight / 2f + characterController.skinWidth, 0f);
            crouchStandHeightDifference = initialHeight - crouchHeight;

            // Initialize camera heights
            initialCameraHeight = cameraTransform.localPosition.y;
            crouchCameraHeight = initialCameraHeight - crouchStandHeightDifference;
        }

        public void HandleCrouch()
        {
            if (!CanToggleCrouch()) return;

            ToggleCrouch();
        }

        bool CanToggleCrouch()
        {
            var isGrounded = collisionController.IsGrounded;
            var isBlockedByRoof = IsCrouching && collisionController.HasRoof;
            return isGrounded && !isBlockedByRoof;
        }

        void ToggleCrouch()
        {
            if (activeCrouchCoroutine != null)
                monoBehaviour.StopCoroutine(activeCrouchCoroutine);

            activeCrouchCoroutine = monoBehaviour.StartCoroutine(CrouchRoutine());
        }

        IEnumerator CrouchRoutine()
        {
            isDuringCrouchAnimation = true;

            var elapsedTime = 0f;
            var duration = movementSettings.crouchTransitionDuration;
            var inverseDuration = 1f / duration;

            // Store initial values
            var startHeight = characterController.height;
            var startCenter = characterController.center;
            var startCameraHeight = cameraTransform.localPosition.y;

            // Determine target values
            var targetHeight = IsCrouching ? initialHeight : crouchHeight;
            var targetCenter = IsCrouching ? initialCenter : crouchCenter;
            var targetCameraHeight = IsCrouching ? initialCameraHeight : crouchCameraHeight;

            // Toggle crouch state
            IsCrouching = !IsCrouching;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = movementSettings.crouchTransitionCurve.Evaluate(elapsedTime * inverseDuration);

                // Smoothly interpolate values
                characterController.height = Helpers.ExpDecay(startHeight, targetHeight, t);
                characterController.center = Helpers.ExpDecay(startCenter, targetCenter, t);

                var cameraPosition = cameraTransform.localPosition;
                cameraPosition.y = Helpers.ExpDecay(startCameraHeight, targetCameraHeight, t);
                cameraTransform.localPosition = cameraPosition;

                yield return null;
            }

            // Ensure final values are set exactly
            characterController.height = targetHeight;
            characterController.center = targetCenter;

            var finalCameraPosition = cameraTransform.localPosition;
            finalCameraPosition.y = targetCameraHeight;
            cameraTransform.localPosition = finalCameraPosition;

            isDuringCrouchAnimation = false;
        }
    }
}