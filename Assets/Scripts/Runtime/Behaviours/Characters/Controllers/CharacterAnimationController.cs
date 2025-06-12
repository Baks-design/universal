using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField, Child] Animator animator;
        [SerializeField, Parent] CharacterMovementController movementController;
        readonly int animIDSpeed = Animator.StringToHash("Speed");
        readonly int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        void Update()
        {
            //animator.SetFloat(animIDSpeed, movementController.CharacterMovement.CalculateBlend);TODO: Adjust
            //animator.SetFloat(animIDMotionSpeed, 1f);
        }
    }
}
