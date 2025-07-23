using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class JumpHandler
    {
        readonly MovementSettings settings;
        readonly CharacterCollisionController collisionController;
        readonly CrouchHandler crouchHandler;

        public JumpHandler(
            MovementSettings settings,
            CharacterCollisionController collisionController,
            CrouchHandler crouchHandler)
        {
            this.settings = settings;
            this.collisionController = collisionController;
            this.crouchHandler = crouchHandler;
        }

        /// <summary>
        /// Attempts to make the character jump if conditions are met
        /// </summary>
        /// <param name="moveVector">Reference to the movement vector to modify</param>
        /// <returns>True if jump was successful, false otherwise</returns>
        public bool TryJump(ref Vector3 moveVector)
        {
            if (!CanJump()) return false;

            ExecuteJump(ref moveVector);

            return true;
        }

        /// <summary>
        /// Applies gravity effects to the movement vector
        /// </summary>
        /// <param name="moveVector">Reference to the movement vector to modify</param>
        public void ApplyGravity(ref Vector3 moveVector)
        {
            if (collisionController.IsGrounded)
                moveVector.y = -settings.stickToGroundForce;
            else
                moveVector += settings.gravityMultiplier * Time.deltaTime * Physics.gravity;
        }

        bool CanJump() => !crouchHandler.IsCrouching && collisionController.IsGrounded;

        void ExecuteJump(ref Vector3 moveVector)
        {
            moveVector.y = settings.jumpSpeed;
            collisionController.IsGrounded = false;
        }
    }
}