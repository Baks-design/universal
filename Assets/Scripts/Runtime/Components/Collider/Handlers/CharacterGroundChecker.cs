using UnityEngine;
using Universal.Runtime.Components.Collider;

public class CharacterGroundChecker
{
    readonly CharacterCollisionData data;
    readonly CharacterController character;

    public bool WasGrounded { get; set; }
    public float SlopeAngle { get; private set; }
    public bool IsGrounded { get; set; }
    public bool IsOnSlope { get; private set; }
    public RaycastHit IsGroundHit { get; private set; }

    public CharacterGroundChecker(CharacterCollisionData data, CharacterController character)
    {
        this.data = data;
        this.character = character;

        WasGrounded = true;
        IsGrounded = true;
    }

    public void CheckGrounded()
    {
        (IsGrounded, IsGroundHit) = GamePhysics.SphereCast(
            character.transform.position, -character.transform.up, character.radius,
            data.floorRayOriginPositionOffset, data.floorRayMaxDistance,
            data.floorCollisionLayers
        );

        WasGrounded = IsGrounded;
        SlopeAngle = Vector3.Angle(IsGroundHit.normal, character.transform.up);
        IsOnSlope = SlopeAngle > 0f;
    }
}
