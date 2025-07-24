using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class MovementSettings
    {
        [Header("Sliding Settings")]
        public float slideSpeed = 5f;

        [Header("Walk Settings")]
        public float walkSpeed = 6f;
        [Range(0f, 1f)] public float moveBackwardsSpeedPercent = 0.5f;
        [Range(0f, 1f)] public float moveSideSpeedPercent = 0.7f;

        [Header("Run Settings")]
        public float runSpeed = 9f;
        [Range(-1f, 1f)] public float canRunThreshold = 0.7f;
        public AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Crouch Settings")]
        public float crouchSpeed = 3f;
        [Range(0.2f, 0.9f)] public float crouchPercent = 0.6f;
        public float crouchTransitionDuration = 0.5f;
        public AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Landing Settings")]
        [Range(0.05f, 0.5f)] public float lowLandAmount = 0.1f;
        [Range(0.2f, 0.9f)] public float highLandAmount = 0.4f;
        public float landTimer = 0.5f;
        public float landDuration = 0.5f;
        public AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Gravity Settings")]
        public float gravityMultiplier = 2.5f;
        public float stickToGroundForce = 1f;

        [Header("Smooth Settings")]
        [Range(1f, 100f)] public float smoothRotateSpeed = 10f;
        [Range(1f, 100f)] public float smoothInputSpeed = 10f;
        [Range(1f, 100f)] public float smoothVelocitySpeed = 3f;
        [Range(1f, 100f)] public float smoothFinalDirectionSpeed = 10f;
        [Range(1f, 100f)] public float smoothHeadBobSpeed = 5f;
    }
}