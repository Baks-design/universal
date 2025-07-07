using System;
using UnityEngine;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class TargetingController : MonoBehaviour, ITargetingSystem
    {
        [SerializeField] LayerMask targetLayer;
        [SerializeField] float maxDistance = 20f;
        Camera playerCamera;
        BodyPartCollection currentTarget;
        int currentSelectionIndex = -1;

        public BodyPart CurrentSelectedPart { get; set; }

        public event Action<BodyPart> OnTargetChanged = delegate { };

        void Awake() => playerCamera = Camera.main;

        public bool TrySelectTarget(out BodyPart targetPart)
        {
            targetPart = null;

            var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out var hit, maxDistance, targetLayer))
            {
                currentTarget = hit.collider.GetComponentInParent<BodyPartCollection>();
                if (currentTarget != null && currentTarget.BodyParts.Count > 0)
                {
                    targetPart = currentTarget.GetBodyPartAtPosition(hit.point);
                    currentSelectionIndex = currentTarget.BodyParts.IndexOf(targetPart);
                    SetCurrentSelectedPart(targetPart);
                    return true;
                }
            }

            SetCurrentSelectedPart(null);
            return false;
        }

        public void CycleSelection(float forward)
        {
            if (currentTarget == null || currentTarget.BodyParts.Count == 0) return;

            var direction = forward > 0f ? 1 : -1;
            currentSelectionIndex = (currentSelectionIndex + direction) % currentTarget.BodyParts.Count;
            if (currentSelectionIndex < 0) currentSelectionIndex = currentTarget.BodyParts.Count - 1;

            SetCurrentSelectedPart(currentTarget.BodyParts[currentSelectionIndex]);
        }

        void SetCurrentSelectedPart(BodyPart part)
        {
            if (CurrentSelectedPart == part) return;

            CurrentSelectedPart = part;
            OnTargetChanged.Invoke(part);
        }
    }
}