using UnityEngine;
using Universal.Runtime.Systems.Stats;

namespace Universal.Runtime.Behaviours
{
    public abstract class Pickup : MonoBehaviour, IVisitor
    {
        protected abstract void ApplyPickupEffect(Entity entity);

        public void Visit<T>(T visitable) where T : Component, IVisitable
        {
            if (visitable is Entity entity)
                ApplyPickupEffect(entity);
        }

        public void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent<IVisitable>(out var visitable);
            visitable.Accept(this);
            Debug.Log($"Picked up {name}");
            Destroy(gameObject);
        }
    }
}