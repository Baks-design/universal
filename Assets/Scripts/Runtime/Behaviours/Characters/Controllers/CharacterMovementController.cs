using UnityEngine;
using KBCore.Refs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Child] CharacterSoundController soundController;
        [SerializeField] Transform cameraHead;
        [SerializeField] CharacterData data;

        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }
        public CharacterCollision CharacterCollision { get; private set; }
        public CharacterHeadBobbing CharacterHeadBobbing { get; private set; }

        void OnEnable()
        {
            CharacterRotation = new CharacterRotation(this, data);
            CharacterMovement = new CharacterMovement(this, data, rb);
            CharacterCollision = new CharacterCollision(this, data);
            CharacterHeadBobbing = new CharacterHeadBobbing(this, data, cameraHead);
        }

        void OnCollisionEnter(Collision collision) => CharacterCollision.CollisionEnter();

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            DrawMovementGizmos();
            DrawCollisionDetectionGizmos();
        }

        void DrawMovementGizmos()
        {
            // Current position marker
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);

            // Target position indicator
            if (CharacterMovement.IsMoving)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(CharacterMovement.TargetPosition, Vector3.one * 0.3f);
                Gizmos.DrawLine(transform.position, CharacterMovement.TargetPosition);

                // Movement path
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange
                Gizmos.DrawSphere(Vector3.Lerp(transform.position, CharacterMovement.TargetPosition,
                    Mathf.Clamp01(Vector3.Distance(
                        transform.position, CharacterMovement.TargetPosition) / data.gridSize)), 0.1f);
            }

            // Grid cell visualization
            Gizmos.color = new Color(0f, 1f, 1f, 0.1f); // Cyan
            Vector3 gridPos = CharacterCollision.SnapToGrid(transform.position);
            Gizmos.DrawWireCube(gridPos + Vector3.up * 0.01f, new Vector3(data.gridSize, 0.02f, data.gridSize));
        }

        void DrawCollisionDetectionGizmos()
        {
            // Obstacle detection visualization
            Gizmos.color = Color.red;
            var halfExtents = new Vector3(0.4f, 0.9f, 0.4f);

            // Draw detection boxes in all potential movement directions
            Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
            foreach (var dir in directions)
            {
                var center = transform.position + 0.5f * data.gridSize * dir + Vector3.up * 0.9f;
                var canMove = !Physics.CheckBox(center, halfExtents, Quaternion.identity);

                Gizmos.color = canMove ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(center, halfExtents * 2f);
                Gizmos.color = canMove ? Color.green : Color.red;
                Gizmos.DrawWireCube(center, halfExtents * 2f);
            }

            // Ground check visualization
            Gizmos.color = CharacterCollision.IsGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(
                transform.position + Vector3.up * 0.1f,
                transform.position + Vector3.up * 0.1f + Vector3.down * data.groundCheckDistance
            );

            // Last Y position marker (for fall distance)
            if (!CharacterCollision.IsGrounded)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(
                    new Vector3(transform.position.x, CharacterCollision.LastYPosition, transform.position.z),
                    Vector3.one * 0.2f
                );
            }
        }
#endif
    }
}