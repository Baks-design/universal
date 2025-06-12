using UnityEngine;

namespace Universal.Runtime.Components.Collider
{
    public static class GamePhysics
    {
        public static (bool isColl, RaycastHit hitInfo) SphereCast(
            Vector3 origin,
            Vector3 direction,
            float radius,
            float offset,
            float maxDistance,
            LayerMask layers,
            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
        {
            var isColl = Physics.SphereCast(
                origin + (direction * offset),
                radius,
                direction,
                out var hitInfo,
                maxDistance,
                layers,
                query
            );
            return (isColl, hitInfo);
        }

        public static (bool isColl, RaycastHit hitInfo) Linecast(
            Vector3 start,
            Vector3 end,
            LayerMask layers,
            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
        {
            var isColl = Physics.Linecast(
                start,
                end,
                out var hitInfo,
                layers,
                query
            );
            return (isColl, hitInfo);
        }
    }
}