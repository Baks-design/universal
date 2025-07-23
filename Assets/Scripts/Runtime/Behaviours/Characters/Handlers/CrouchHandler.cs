using System.Collections;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Behaviours.Characters
{
    using UnityEngine;
    using System.Collections;

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
        bool receivedCrouchInput;

        public bool IsCrouching { get; private set; }
        public bool DuringCrouchAnimation => isDuringCrouchAnimation;
        public float InitialCameraHeight => initialCameraHeight;
        public float CrouchCameraHeight => crouchCameraHeight;

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

        /// <summary>
        /// Call this from the new Input System's performed callback
        /// </summary>
        public void OnCrouchInput() => receivedCrouchInput = true;

        /// <summary>
        /// Should be called every frame to process crouch logic
        /// </summary>
        public void UpdateCrouch()
        {
            if (!receivedCrouchInput) return;
            receivedCrouchInput = false;

            if (!CanToggleCrouch()) return;

            ToggleCrouch();
        }

        bool CanToggleCrouch()
        {
            var isGrounded = collisionController.IsGrounded;
            var isBlockedByRoof = IsCrouching && collisionController.HasRoof;
            return isGrounded && !isBlockedByRoof && !isDuringCrouchAnimation;
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
                characterController.height = Mathf.Lerp(startHeight, targetHeight, t);
                characterController.center = Vector3.Lerp(startCenter, targetCenter, t);

                var cameraPosition = cameraTransform.localPosition;
                cameraPosition.y = Mathf.Lerp(startCameraHeight, targetCameraHeight, t);
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