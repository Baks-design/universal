using UnityEngine;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    // public class CharacterMovement //TODO: Remove
    // {
    //     readonly Transform transform;
    //     readonly CharacterController controller;
    //     readonly CharacterData data;
    //     Vector3 movement;
    //     float targetRotation;
    //     float speed;
    //     float rotationVelocity;
    //     float currentAnimationBlend;
    //     const float GroundedVerticalVelocity = -2f;
    //     const float SpeedThreshold = 0.1f;

    //     public bool IsMoving => controller.velocity.sqrMagnitude > 0.1f;
    //     public float CalculateBlend
    //     {
    //         get
    //         {
    //             currentAnimationBlend = Mathf.Lerp(currentAnimationBlend, speed, Time.deltaTime * data.speedChangeRate);
    //             return currentAnimationBlend < 0.01f ? 0f : currentAnimationBlend;
    //         }
    //     }

    //     public CharacterMovement(Transform transform, CharacterController controller, CharacterData data)
    //     {
    //         this.transform = transform;
    //         this.controller = controller;
    //         this.data = data;
    //     }

    //     public void HandleMovement(Vector2 input, float deltaTime)
    //     {
    //         ProcessRotation();
    //         CalculateSpeed(input, deltaTime);
    //         CalculateDirection(deltaTime);
    //         CalculateGravity(deltaTime);
    //         ProcessMovement();
    //     }

    //     void ProcessRotation()
    //     {
    //         var input = PlayerMapInputProvider.Move;
    //         if (input != Vector2.zero)
    //         {
    //             var inputDirection = new Vector3(input.x, 0f, input.y).normalized;
    //             targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
    //             var rotation = Mathf.SmoothDampAngle(
    //                 transform.eulerAngles.y,
    //                 targetRotation,
    //                 ref rotationVelocity,
    //                 data.rotationSmoothTime);
    //             transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    //         }
    //     }

    //     void CalculateSpeed(Vector2 input, float deltaTime)
    //     {
    //         var targetSpeed = input == Vector2.zero ? 0f : data.movementSpeed;

    //         var currentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
    //         if (Mathf.Abs(currentHorizontalSpeed - targetSpeed) > SpeedThreshold)
    //         {
    //             speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, deltaTime * data.speedChangeRate);
    //             speed = Mathf.Round(speed * 1000f) / 1000f;
    //         }
    //         else
    //             speed = targetSpeed;
    //     }

    //     void CalculateDirection(float deltaTime)
    //     {
    //         var targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
    //         movement = targetDirection.normalized * (speed * deltaTime);
    //     }

    //     void CalculateGravity(float deltaTime)
    //     {
    //         var verticalVelocity = controller.isGrounded ? GroundedVerticalVelocity : controller.velocity.y;
    //         verticalVelocity += Physics.gravity.y * deltaTime;
    //         movement.y = verticalVelocity * deltaTime;
    //     }

    //     void ProcessMovement() => controller.Move(movement);
    // }
}
