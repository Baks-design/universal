using System.Collections;
using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCrouch
    {
        readonly CharacterMovementController controller;
        readonly CapsuleCollider collider;
        readonly CharacterData data;
        readonly Transform cameraTransform;
        readonly Vector3 initCenter;
        readonly Vector3 crouchCenter;
        readonly float crouchHeight;
        readonly float initCamHeight;
        readonly float crouchCamHeight;
        readonly float initHeight;
        Coroutine crouchCoroutine;

        public bool IsCrouching { get; private set; }

        public CharacterCrouch(
            CharacterMovementController controller,
            CapsuleCollider collider,
            CharacterData data,
            Transform cameraTransform)
        {
            this.controller = controller;
            this.collider = collider;
            this.data = data;
            this.cameraTransform = cameraTransform;

            IsCrouching = false;

            collider.center = new Vector3(0f, collider.height / 2f, 0f);

            initHeight = collider.height;
            initCenter = collider.center;
            initCamHeight = cameraTransform.localPosition.y;

            crouchHeight = Clamp01(initHeight * data.crouchCameraHeightRatio);
            crouchCenter = crouchHeight / 2f * Vector3.up;

            var crouchStandHeightDifference = initHeight - crouchHeight;
            crouchCamHeight = Clamp01(initCamHeight - crouchStandHeightDifference);
        }

        public void HandleCrouchInput()
        {
            if (crouchCoroutine != null)
                controller.StopCoroutine(crouchCoroutine);

            crouchCoroutine = controller.StartCoroutine(SmoothCrouchTransition());
        }

        IEnumerator SmoothCrouchTransition()
        {
            var startHeight = collider.height;
            var startCenter = collider.center;
            var startCamHeight = cameraTransform.localPosition.y;

            var targetHeight = IsCrouching ? initHeight : crouchHeight;
            var targetCenter = IsCrouching ? initCenter : crouchCenter;
            var targetCamHeight = IsCrouching ? initCamHeight : crouchCamHeight;

            IsCrouching = !IsCrouching;

            var elapsed = 0f;
            var speed = data.crouchTransitionSpeed > 0f ? 1f / data.crouchTransitionSpeed : 1f;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * speed;

                collider.height = Lerp(startHeight, targetHeight, elapsed);
                collider.center = Vector3.Lerp(startCenter, targetCenter, elapsed);
                controller.CharacterHeadBobbing.AdjustBaseHeight(Lerp(startCamHeight, targetCamHeight, elapsed));

                yield return null;
            }

            collider.height = targetHeight;
            collider.center = targetCenter;
            controller.CharacterHeadBobbing.AdjustBaseHeight(targetCamHeight);
        }
    }
}