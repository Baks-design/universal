using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class GamePhysics
    {
        public static (bool isColl, RaycastHit hitInfo) SphereCast(
            Vector3 origin, Vector3 direction, float radius, float offset, float maxDistance, LayerMask layers)
        {
            var isColl = Physics.SphereCast(
                origin + (direction * offset), radius, direction, out var hitInfo,
                maxDistance, layers, QueryTriggerInteraction.Ignore
            );
            return (isColl, hitInfo);
        }

        public static (bool isColl, RaycastHit hitInfo) Linecast(
            Vector3 start, Vector3 end, LayerMask layers)
        {
            var isColl = Physics.Linecast(
                start, end, out var hitInfo, layers, QueryTriggerInteraction.Ignore
            );
            return (isColl, hitInfo);
        }
    }
}