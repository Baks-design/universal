using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class GravityHandler
    {
        readonly MovementSettings settings;
        readonly CharacterCollisionController collision;

        public GravityHandler(MovementSettings settings, CharacterCollisionController collision)
        {
            this.settings = settings;
            this.collision = collision;
        }

        /// <summary>
        /// Applies gravity effects to the movement vector
        /// </summary>
        /// <param name="moveVector">Reference to the movement vector to modify</param>
        public void ApplyGravity(ref Vector3 moveVector)
        {
            if (collision.IsGrounded)
                moveVector.y = -settings.stickToGroundForce;
            else
                moveVector += settings.gravityMultiplier * Time.deltaTime * Physics.gravity;
        }
    }
}