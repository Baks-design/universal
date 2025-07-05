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
        readonly float initCamHeight;
        readonly float crouchCamHeight;
        readonly float initHeight;
        Coroutine crouchCoroutine;
        Vector3 initCenter;
        Vector3 crouchCenter;

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

            initCenter = collider.center;
            initHeight = collider.height;

            var crouchHeight = initHeight * data.crouchCameraHeightRatio;
            crouchCenter = crouchHeight / 2f * Vector3.up;

            var crouchStandHeightDifference = initHeight - crouchHeight;
            initCamHeight = cameraTransform.localPosition.y;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;
        }

        public void HandleCrouchInput()
        {
            if (crouchCoroutine != null)
            {
                controller.StopCoroutine(crouchCoroutine);
                crouchCoroutine = null;
            }

            IsCrouching = !IsCrouching;

            crouchCoroutine = controller.StartCoroutine(SmoothCrouchTransition());
        }

        IEnumerator SmoothCrouchTransition()
        {
            var elapsed = 0f;
            var speed = 1f / data.crouchTransitionSpeed;

            var targetColliderHeight = IsCrouching ? initHeight : data.crouchHeight;
            var targetColliderCenter = IsCrouching ? initCenter : crouchCenter;

            var targetCameraHeight = IsCrouching ? crouchCamHeight: initCamHeight;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * speed;

                // Update collider
                collider.height = Lerp(collider.height, targetColliderHeight, elapsed);
                collider.center = Vector3.Lerp(collider.center, targetColliderCenter, elapsed);

                // Update camera height
                controller.CharacterHeadBobbing.AdjustBaseHeight(
                    Lerp(cameraTransform.position.y, targetCameraHeight, elapsed)
                );

                yield return null;
            }
        }
    }
}