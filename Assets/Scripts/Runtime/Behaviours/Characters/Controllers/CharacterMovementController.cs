using UnityEngine;
using KBCore.Refs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField, Self] CapsuleCollider coll;
        [SerializeField] Transform cameraHead;
        [SerializeField] CharacterData data;

        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }
        public CharacterCollision CharacterCollision { get; private set; }
        public CharacterHeadBob CharacterHeadBobbing { get; private set; }
        public CharacterCrouch CharacterCrouch { get; private set; }

        void Awake()
        {
            CharacterRotation = new CharacterRotation(this, data);
            CharacterMovement = new CharacterMovement(this, data);
            CharacterCollision = new CharacterCollision(this, data, coll, Camera.main);
            CharacterHeadBobbing = new CharacterHeadBob(this, data, cameraHead);
            CharacterCrouch = new CharacterCrouch(this, coll, data, cameraHead);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            CharacterCollision.DrawCeilingCheckGizmos();
            CharacterCollision.DrawDetectionRayGizmos();
            CharacterCollision.DrawGroundCheckGizmos();
            CharacterCollision.DrawMovementGizmos();
        }
#endif
    }
}