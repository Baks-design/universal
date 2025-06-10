using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField, Child] Animator animator;
        [SerializeField, Parent] CharacterSwitchController switchController;
        readonly int animIDSpeed = Animator.StringToHash("Speed");
        readonly int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        void Update()
        {
            animator.SetFloat(
                animIDSpeed,
                switchController.MovementController.CharacterMovement.CalculateBlend());
            animator.SetFloat(animIDMotionSpeed, 1f);
        }
    }
}
