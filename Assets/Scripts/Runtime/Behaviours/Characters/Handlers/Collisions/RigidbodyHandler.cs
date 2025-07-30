using System.Collections.Generic;
using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class RigidbodyHandler
    {
        readonly CharacterController controller;
        readonly PhysicsSettings settings;
        readonly List<Rigidbody> cachedKeysList = new();
        readonly Dictionary<Rigidbody, float> cooldownTimers = new();

        public RigidbodyHandler(CharacterController controller, PhysicsSettings settings)
        {
            this.controller = controller;
            this.settings = settings;
        }

        public void UpdateCooldowns()
        {
            if (!settings.enablePushCooldown || cooldownTimers.Count == 0) return;

            var expiredEntries = new List<Rigidbody>();
            cachedKeysList.Clear();
            cachedKeysList.AddRange(cooldownTimers.Keys);

            for (var i = 0; i < cachedKeysList.Count; i++)
            {
                var key = cachedKeysList[i];
                var timer = cooldownTimers[key] - Time.deltaTime;
                cooldownTimers[key] = timer;
                if (timer <= 0f) expiredEntries.Add(key);
            }

            for (var i = 0; i < expiredEntries.Count; i++)
                cooldownTimers.Remove(expiredEntries[i]);
        }

        public void PushRigidbody(ControllerColliderHit hit)
        {
            if (hit == null) return;

            var body = hit.collider.attachedRigidbody;
            if (!CanPush(body)) return;

            var pushDir = CalculatePushDirection(hit);
            var finalPower = GetFinalPushPower(body.mass);

            ApplyPushForces(body, pushDir, finalPower);
            TryApplyTorque(body, pushDir);

            if (settings.enablePushCooldown)
                cooldownTimers[body] = settings.cooldownTime;
        }

        bool CanPush(Rigidbody body)
        {
            if (body == null || body.isKinematic) return false;
            if (!Helpers.IsInLayerMask(body.gameObject.layer, settings.pushableLayers)) return false;
            if (settings.enablePushCooldown && cooldownTimers.ContainsKey(body)) return false;
            return true;
        }

        Vector3 CalculatePushDirection(ControllerColliderHit hit)
        {
            var pushDir = settings.useCharacterForward && controller.transform != null
                ? controller.transform.forward
                : hit.moveDirection;
            pushDir.y *= settings.verticalInfluence;
            return pushDir.normalized;
        }

        float GetFinalPushPower(float bodyMass)
        {
            var power = settings.pushPower;
            if (settings.scalePowerByMass)
                power *= Clamp(bodyMass, settings.minMass, settings.maxMass);
            return power;
        }

        void ApplyPushForces(Rigidbody body, Vector3 direction, float power)
        {
            body.AddForce(direction * power, settings.pushForceMode);

            if (settings.maxPushedVelocity > 0f)
            {
                var horizontalVel = body.linearVelocity;
                horizontalVel.y = 0f;
                if (horizontalVel.magnitude > settings.maxPushedVelocity)
                {
                    horizontalVel = horizontalVel.normalized * settings.maxPushedVelocity;
                    body.linearVelocity = new Vector3(horizontalVel.x, body.linearVelocity.y, horizontalVel.z);
                }
            }
        }

        void TryApplyTorque(Rigidbody body, Vector3 pushDir)
        {
            if (!settings.applyTorque) return;

            var torque = new Vector3(
                pushDir.z * settings.torqueMultiplier,
                0f,
                -pushDir.x * settings.torqueMultiplier
            );
            body.AddTorque(torque, settings.pushForceMode);
        }
    }
}