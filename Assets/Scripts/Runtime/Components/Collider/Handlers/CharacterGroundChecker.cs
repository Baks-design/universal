using UnityEngine;

namespace Universal.Runtime.Components.Collider
{
    public class CharacterGroundChecker
    {
        readonly CharacterCollisionData data;
        readonly Transform bodyTransform;
        RaycastHit hitInfo;
        
        public bool WasGrounded { get; set; }
        public float SlopeAngle { get; private set; }
        public bool IsGrounded { get; set; }
        public bool IsOnSlope { get; private set; }
        public RaycastHit IsGroundHit => hitInfo;

        public CharacterGroundChecker(CharacterCollisionData data, Transform bodyTransform)
        {
            this.data = data;
            this.bodyTransform = bodyTransform;

            WasGrounded = true;
            IsGrounded = true;
        }

        public void CheckGrounded()
        {
            if (Physics.Raycast(
                bodyTransform.transform.position,
                -bodyTransform.up,
                out hitInfo,
                data.floorRayMaxDistance,
                data.floorCollisionLayers))
            {
                WasGrounded = IsGrounded;
                SlopeAngle = Vector3.Angle(hitInfo.normal, bodyTransform.transform.up);
                IsOnSlope = SlopeAngle > 0f;
            }
        }
    }
}