using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.Updates;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour, IUpdatable
    {
        [SerializeField, Self] CharacterController controller;
        [SerializeField, Self] CharacterMovementController movement;
        [SerializeField] PhysicsSettings settings;
        GroundHandler groundHandler;
        ObstacleHandler obstacleHandler;
        RoofHandler roofHandler;
        RigidbodyHandler rigidbodyHandler;

        //Ground
        public bool IsGrounded
        {
            get => groundHandler.IsGrounded;
            set => groundHandler.IsGrounded = value;
        }
        public bool JustLanded => groundHandler.JustLanded;
        public RaycastHit GroundHit => groundHandler.HitInfo;
        public bool IsOnSteepSlope => groundHandler.IsOnSteepSlope;
        public float SlopeAngle => groundHandler.SlopeAngle;
        public Vector3 SlopeDirection => groundHandler.GetSlopeSlideDirection();
        //Obstacle
        public bool HasObstacle => obstacleHandler.HasObstacle;
        public RaycastHit ObstacleHit => obstacleHandler.ObstacleHit;
        //Roof
        public bool HasRoof => roofHandler.HasRoof;

        void Awake()
        {
            groundHandler = new GroundHandler(controller, settings);
            obstacleHandler = new ObstacleHandler(controller, settings, movement);
            roofHandler = new RoofHandler(controller, settings);
            rigidbodyHandler = new RigidbodyHandler(controller, settings);
        }

        void OnEnable() => this.AutoRegisterUpdates();

        void OnDisable() => this.AutoUnregisterUpdates();

        public void OnUpdate()
        {
            groundHandler.CheckGround();
            obstacleHandler.CheckObstacle();
            roofHandler.CheckIfRoof();
            rigidbodyHandler.UpdateCooldowns();
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => rigidbodyHandler.PushRigidbody(hit);

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            groundHandler.DrawGroundCheckGizmo();
            obstacleHandler.DrawObstacleCheckGizmo();
            roofHandler.DrawRoofCheckGizmo();
        }
    }
}