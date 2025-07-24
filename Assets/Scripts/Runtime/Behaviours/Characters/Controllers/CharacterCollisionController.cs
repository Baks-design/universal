using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour
    {
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Self] CharacterMovementController movement;
        [SerializeField] PhysicsSettings settings;
        GroundChecker groundChecker;
        ObstacleChecker obstacleChecker;
        RoofChecker roofChecker;

        //Ground
        public bool IsGrounded
        {
            get => groundChecker.IsGrounded;
            set => groundChecker.IsGrounded = value;
        }
        public bool JustLanded => groundChecker.JustLanded;
        public RaycastHit GroundHit => groundChecker.HitInfo;
        public bool IsOnSteepSlope => groundChecker.IsOnSteepSlope;
        public float SlopeAngle => groundChecker.SlopeAngle;
        public Vector3 SlopeDirection => groundChecker.GetSlopeSlideDirection();
        //Obstacle
        public bool HasObstacle => obstacleChecker.HasObstacle;
        public RaycastHit ObstacleHit => obstacleChecker.ObstacleHit;
        //Roof
        public bool HasRoof => roofChecker.HasRoof;

        void Awake()
        {
            groundChecker = new GroundChecker(controller, settings);
            obstacleChecker = new ObstacleChecker(controller, settings, movement);
            roofChecker = new RoofChecker(controller, settings);
        }

        void Update()
        {
            groundChecker.CheckGround();
            obstacleChecker.CheckObstacle();
            roofChecker.CheckIfRoof();
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            groundChecker.DrawGroundCheckGizmo();
            obstacleChecker.DrawObstacleCheckGizmo();
            roofChecker.DrawRoofCheckGizmo();
        }
    }
}

// TODO: Add rigidbody collisions