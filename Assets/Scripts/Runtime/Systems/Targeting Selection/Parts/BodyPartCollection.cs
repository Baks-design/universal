using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class BodyPartCollection : MonoBehaviour
    {
        [SerializeField] List<BodyPart> bodyParts = new();

        public List<BodyPart> BodyParts => bodyParts;

        void Awake()
        {
            if (bodyParts.Count == 0)
                bodyParts.AddRange(GetComponentsInChildren<BodyPart>());
        }

        public BodyPart GetBodyPartAtPosition(Vector3 position)
        {
            BodyPart closestPart = null;
            var closestDistance = Mathf.Infinity;

            for (var i = 0; i < bodyParts.Count; i++)
            {
                var part = bodyParts[i];
                if (part == null || part.PartCollider == null) continue;

                // First check if point is inside collider
                if (part.PartCollider.bounds.Contains(position)) return part;

                // Otherwise get closest point on collider
                var closestPoint = part.PartCollider.ClosestPoint(position);
                var distance = Vector3.Distance(closestPoint, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPart = part;
                }
            }

            return closestPart;
        }
    }
}