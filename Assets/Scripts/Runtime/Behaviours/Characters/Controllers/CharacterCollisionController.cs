using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Collider;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour
    {
        [SerializeField, Parent] CharacterController character;
        [SerializeField, InLineEditor] CharacterCollisionData data;

        public CharacterController Character => character;
        public CharacterGroundChecker GroundChecker { get; private set; }

        void Awake() => GroundChecker = new CharacterGroundChecker(data, character);

        void Update() => GroundChecker.CheckGrounded();

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            DebugGroundCheck();
        }

        void DebugGroundCheck()
        {
            var spherePos = new Vector3(
                character.transform.position.x,
                character.transform.position.y - data.floorRayOriginPositionOffset,
                character.transform.position.z
            );

            Gizmos.color = GroundChecker.IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(spherePos, character.radius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spherePos, spherePos + -character.transform.up * data.floorRayMaxDistance);

            if (GroundChecker.IsGrounded)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(GroundChecker.IsGroundHit.point, 0.05f);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(GroundChecker.IsGroundHit.point, GroundChecker.IsGroundHit.normal * 0.5f);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(spherePos, GroundChecker.IsGroundHit.point);
            }
        }
    }
}