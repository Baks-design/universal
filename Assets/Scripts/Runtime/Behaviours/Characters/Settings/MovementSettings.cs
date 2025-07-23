using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class MovementSettings
    {
        [Header("Locomotion Settings")]
        public float crouchSpeed = 1f;
        public float walkSpeed = 2f;
        public float runSpeed = 3f;
        public float jumpSpeed = 5f;
        [Range(0f, 1f)] public float moveBackwardsSpeedPercent = 0.5f;
        [Range(0f, 1f)] public float moveSideSpeedPercent = 0.75f;

        [Header("Run Settings")]
        [Range(-1f, 1f)] public float canRunThreshold = 0.8f;
        public AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Crouch Settings")]
        [Range(0.2f, 0.9f)] public float crouchPercent = 0.6f;
        public float crouchTransitionDuration = 1f;
        public AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Landing Settings")]
        [Range(0.05f, 0.5f)] public float lowLandAmount = 0.1f;
        [Range(0.2f, 0.9f)] public float highLandAmount = 0.6f;
        public float landTimer = 0.5f;
        public float landDuration = 1f;
        public AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Gravity Settings")]
        public float gravityMultiplier = 2.5f;
        public float stickToGroundForce = 5f;

        [Header("Smooth Settings")]
        [Range(1f, 100f)] public float smoothRotateSpeed = 5f;
        [Range(1f, 100f)] public float smoothInputSpeed = 5f;
        [Range(1f, 100f)] public float smoothVelocitySpeed = 5f;
        [Range(1f, 100f)] public float smoothFinalDirectionSpeed = 5f;
        [Range(1f, 100f)] public float smoothHeadBobSpeed = 5f;
    }
}