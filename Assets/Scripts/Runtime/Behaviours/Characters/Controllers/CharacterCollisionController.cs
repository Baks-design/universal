using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, InlineEditor] CharacterData data;
        Transform cameraTransform;
        RaycastHit groundHit;
        RaycastHit characterHit;
        bool wasGrounded;

        public RaycastHit GroundHit => groundHit;
        public bool IsGrounded { get; private set; }
        public bool JustLanded { get; private set; }
        public RaycastHit CharacterHit => characterHit;
        public bool HasCharacterHit { get; private set; }

        void Awake() => cameraTransform = Camera.main.transform;

        void Update()
        {
            UpdateGroundDetection();
            UpdateCharacterDetection();
        }

        void UpdateGroundDetection()
        {
            wasGrounded = IsGrounded;

            IsGrounded = Physics.Raycast(
                tr.localPosition,
                Vector3.down,
                out groundHit,
                data.groundCheckDistance,
                data.groundLayer,
                QueryTriggerInteraction.Ignore);

            JustLanded = !wasGrounded && IsGrounded;
        }

        void UpdateCharacterDetection()
        {
            HasCharacterHit = Physics.SphereCast(
                cameraTransform.localPosition,
                data.detectionRadius,
                cameraTransform.forward,
                out characterHit,
                data.detectionDistance,
                data.detectionLayer,
                QueryTriggerInteraction.Ignore);
        }

        public bool IsPositionFree(Vector3 position)
        {
            var colliders = Physics.OverlapBox(
                position + Vector3.up * (data.collisionCheckSize.y / 2f + data.obstacleYOffset),
                data.collisionCheckSize / 2f,
                tr.localRotation,
                data.obstacleLayers,
                QueryTriggerInteraction.Ignore);
            return colliders.Length == 0 || (colliders.Length == 1 && colliders[0].transform == tr);
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
            Gizmos.DrawRay(tr.localPosition + Vector3.up, Vector3.down * data.groundCheckDistance);

            if (IsGrounded)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(groundHit.point, 0.1f);
            }
        }

        void DrawCharacterDetectionGizmo()
        {
            var rayOrigin = cameraTransform.localPosition;
            var oldMatrix = Gizmos.matrix;

            Gizmos.color = HasCharacterHit ? Color.green : Color.red;
            Gizmos.DrawRay(rayOrigin, cameraTransform.forward * data.detectionDistance);

            Gizmos.matrix = Matrix4x4.TRS(
                rayOrigin + cameraTransform.forward * data.detectionDistance,
                cameraTransform.localRotation,
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
            var oldMatrix = Gizmos.matrix;

            Gizmos.color = IsPositionFree(tr.localPosition) ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(center, tr.localRotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, data.collisionCheckSize);
            Gizmos.matrix = oldMatrix;
        }
    }
}