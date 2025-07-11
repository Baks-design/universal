using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField] ThrowConfiguration throwConfiguration;

        public bool CanBeThrown => rb != null && throwConfiguration != null;

        public void Throw(Vector3 direction, float forceMultiplier = 1f)
        {
            if (!CanBeThrown) return;

            rb.isKinematic = false;
            rb.useGravity = throwConfiguration.useGravity;
            rb.freezeRotation = !throwConfiguration.allowRotation;

            var force = forceMultiplier * throwConfiguration.baseForce * direction.normalized;

            rb.AddForce(force, throwConfiguration.forceMode);
        }

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Hit {collision.gameObject.name} with velocity: {rb.linearVelocity.magnitude}");
        }
    }
}