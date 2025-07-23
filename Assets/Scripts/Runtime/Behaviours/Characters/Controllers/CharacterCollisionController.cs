using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour
    {
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Self] CharacterMovementController movementController;
        [SerializeField] PhysicsSettings settings;
        GroundChecker groundChecker;
        ObstacleChecker obstacleChecker;
        RoofChecker roofChecker;

        public bool IsGrounded
        {
            get => groundChecker.IsGrounded;
            set => groundChecker.IsGrounded = value;
        }
        public bool JustLanded => groundChecker.JustLanded;
        public RaycastHit GroundHit => groundChecker.HitInfo;
        public bool HasObstacle => obstacleChecker.HasObstacle;
        public RaycastHit ObstacleHit => obstacleChecker.ObstacleHit;
        public bool HasRoof => roofChecker.HasRoof;

        void Awake()
        {
            groundChecker = new GroundChecker(controller, settings);
            obstacleChecker = new ObstacleChecker(controller, settings);
            roofChecker = new RoofChecker(controller, settings);
        }

        void Update()
        {
            groundChecker.CheckGround();
            obstacleChecker.CheckObstacle(movementController.Direction);
            roofChecker.CheckIfRoof();
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            groundChecker.DrawGroundCheckGizmo();
            obstacleChecker.DrawObstacleCheckGizmo(movementController.Direction);
            roofChecker.DrawRoofCheckGizmo();
        }
    }
}