using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour //TODO: Adjust
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, InlineEditor] CharacterData data;
        Transform cameraT;
        RaycastHit groundHit;
        RaycastHit characterHit;
        bool wasGrounded;

        public RaycastHit GroundHit => groundHit;
        public bool IsGrounded { get; set; }
        public bool JustLanded { get; set; }
        public RaycastHit CharacterHit => characterHit;
        public bool HasCharacterHit { get; set; }

        void Awake()
        {
            cameraT = Camera.main.transform;
            JustLanded = false;
            IsGrounded = true;
        }

        void Update()
        {
            UpdateGroundDetection();
            UpdateCharacterDetection();
        }

        void UpdateGroundDetection()
        {
            wasGrounded = IsGrounded;
            var rayStart = tr.localPosition + new Vector3(0f, data.groundCheckOffset, 0f);
            IsGrounded = Physics.Raycast(
                rayStart,
                Vector3.down,
                out groundHit,
                data.groundCheckDistance,
                data.groundLayer,
                QueryTriggerInteraction.Ignore);
            JustLanded = !wasGrounded && IsGrounded;
        }

        void UpdateCharacterDetection()
        {
            var ray = new Ray(cameraT.localPosition, cameraT.forward);
            HasCharacterHit = Physics.SphereCast(
                ray,
                data.detectionRadius,
                out characterHit,
                data.detectionDistance,
                data.detectionLayer,
                QueryTriggerInteraction.Ignore);
        }

        public bool IsPositionFree(Vector3 position)
        {
            var center = position + Vector3.up * (data.collisionCheckSize.y / 2f + data.obstacleYOffset);
            var halfExtents = data.collisionCheckSize / 2f;
            var colliders = Physics.OverlapBox(
                center,
                halfExtents,
                tr.localRotation,
                data.obstacleLayers,
                QueryTriggerInteraction.Ignore);
            return colliders.Length == 0;
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            DrawGroundCheckGizmo();
            DrawCharacterDetectionGizmo();
            DrawPositionCheckGizmo();
        }

        void DrawGroundCheckGizmo()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            
            var rayStart = tr.localPosition + new Vector3(0f, data.groundCheckOffset, 0f);
            Gizmos.DrawRay(rayStart, Vector3.down * data.groundCheckDistance);

            if (IsGrounded)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(groundHit.point, 0.1f);
            }
        }

        void DrawCharacterDetectionGizmo()
        {
            Gizmos.color = HasCharacterHit ? Color.cyan : Color.yellow;
            var rayOrigin = cameraT.localPosition;
            Gizmos.DrawRay(rayOrigin, cameraT.forward * data.detectionDistance);

            // Draw sphere at detection distance
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(
                rayOrigin + cameraT.forward * data.detectionDistance,
                cameraT.localRotation,
                2f * data.detectionRadius * Vector3.one
            );
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
            Gizmos.matrix = oldMatrix;

            if (HasCharacterHit)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(characterHit.point, Vector3.one * 0.15f);
            }
        }

        void DrawPositionCheckGizmo()
        {
            var center = tr.localPosition + Vector3.up * (data.collisionCheckSize.y / 2f + data.obstacleYOffset);
            Gizmos.color = IsPositionFree(tr.localPosition) ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.25f);
            Gizmos.matrix = Matrix4x4.TRS(center, tr.localRotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, data.collisionCheckSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}