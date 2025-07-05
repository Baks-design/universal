using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableObject : MonoBehaviour
    {
        [SerializeField, Self] Rigidbody rb;
        [SerializeField] ThrowConfiguration throwConfiguration;

        public bool CanBeThrown => rb != null;

        public void Throw(Vector3 trajectory)
        {
            if (!CanBeThrown) return;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.freezeRotation = true;
            rb.linearVelocity = trajectory;
        }

        void OnCollisionEnter(Collision collision)
        {
            
        }

    }
}