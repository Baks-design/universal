using UnityEngine;
using Universal.Runtime.Components.Collider;

public class CharacterRigidBodyPushHandler
{
    readonly CharacterCollisionData data;

    public CharacterRigidBodyPushHandler(CharacterCollisionData data) => this.data = data;

    public void PushRigidBodies(ControllerColliderHit hit)
    {
        if (!hit.collider.TryGetComponent(out Rigidbody body) || body.isKinematic) return;

        var bodyLayer = body.gameObject.layer;
        if ((data.pushCollsionLayers.value & (1 << bodyLayer)) == 0) return;

        if (hit.moveDirection.y < -0.3f) return;

        var pushDir = hit.moveDirection;
        pushDir.y = 0f;

        body.AddForce(pushDir * data.strength, ForceMode.Impulse);
    }
}
