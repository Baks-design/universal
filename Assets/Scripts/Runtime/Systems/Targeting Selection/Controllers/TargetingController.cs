using System;
using UnityEngine;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class TargetingController : MonoBehaviour, ITargetingSystem
    {
        [SerializeField] LayerMask targetLayer = Physics.AllLayers;
        [SerializeField] float maxDistance = 20f;
        Transform mainCam;
        BodyPartCollection currentTarget;
        int currentSelectionIndex = -1;

        public BodyPart CurrentSelectedPart { get; private set; }

        public event Action<BodyPart> OnTargetChanged = delegate { };

        void Awake() => mainCam = Camera.main.transform;

        public bool TrySelectTarget(out BodyPart targetPart)
        {
            targetPart = null;
            var ray = new Ray(mainCam.localPosition, mainCam.forward);

            if (Physics.SphereCast(ray, maxDistance, out var hit, maxDistance, targetLayer, QueryTriggerInteraction.Ignore))
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