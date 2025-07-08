using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CollisionChecker
    {
        readonly CharacterData data;
        readonly Transform transform;
        readonly Camera camera;
        RaycastHit groundHit;
        RaycastHit characterHit;
        bool wasGrounded;

        public RaycastHit GroundHit => groundHit;
        public bool IsGrounded { get; private set; }
        public bool JustLanded { get; private set; }
        public RaycastHit CharacterHit => characterHit;
        public bool HasCharacter { get; private set; }

        public CollisionChecker(CharacterData data, Transform transform, Camera camera)
        {
            this.data = data;
            this.transform = transform;
            this.camera = camera;
        }

        public bool IsPositionFree(Vector3 position)
        {
            var center = position + Vector3.up * (data.collisionCheckSize.y / 2f);
            var colliders = Physics.OverlapBox(
                center, data.collisionCheckSize / 2f, transform.localRotation, data.obstacleLayers);
            return colliders.Length == 0;
        }

        public void UpdateGroundDetect()
        {
            wasGrounded = IsGrounded;
            if (Physics.Raycast(
                transform.localPosition, Vector3.down, out groundHit, data.groundCheckDistance, data.groundLayer))
                JustLanded = !wasGrounded && IsGrounded;
        }

        public void UpdataCharacterDetect()
        {
            var ray = new Ray(camera.transform.localPosition, camera.transform.forward);
            HasCharacter = Physics.Raycast(ray, out characterHit, data.interactDistance, data.interactableLayer);
        }
    }
}