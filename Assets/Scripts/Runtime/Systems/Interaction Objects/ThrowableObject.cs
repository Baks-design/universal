using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.DamageObjects;

namespace Universal.Runtime.Systems.InteractionObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableObject : DamageSource
    {
        [SerializeField, Self] Rigidbody rb;
        //[SerializeField] VisualEffect impactEffect;
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
            if ((damageableLayers.value & (1 << collision.gameObject.layer)) == 0) return;

            var contact = collision.contacts[0];
            if (contact.otherCollider.TryGetComponent(out IDamageable damageable))
            {
                var result = ApplyDamage(damageable, contact.point, contact.normal);
                HandleDamageResult(result, contact.point, contact.normal);
            }

            // if (impactEffect != null)
            //     Addressables
            //         .InstantiateAsync(impactEffect, contact.point, Quaternion.LookRotation(contact.normal))
            //         .WaitForCompletion();

            // Apply physics force
            if (contact.otherCollider.TryGetComponent<Rigidbody>(out var rb))
                rb.AddForceAtPosition(contact.normal * -throwConfiguration.Force, contact.point);

            Destroy(gameObject);
        }

        void HandleDamageResult(DamageResult result, Vector3 position, Vector3 normal)
        {
            if (result.WasLimbSevered)
            {
                // Play special severing effects
            }
        }
    }
}