using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterAnimationController : MonoBehaviour 
    {
        [SerializeField, Child] Animator animator;
        [SerializeField, Parent] CharacterMovementController controller;
        int animIDSpeed;
        float currentVelocity;

        void Start() => animIDSpeed = Animator.StringToHash("Speed");

        void Update()
        {
            currentVelocity = Mathf.Lerp(
                currentVelocity, controller.Character.velocity.magnitude, Time.deltaTime * 10f);
            animator.SetFloat(animIDSpeed, currentVelocity);
        }
    }
}
