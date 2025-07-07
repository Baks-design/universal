using UnityEngine;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class OutlineHighlightController : MonoBehaviour, IHighlightController
    {
        IHighlightable currentHighlight;

        public void SetHighlight(IHighlightable target)
        {
            if (currentHighlight == target) return;

            ClearHighlight();

            currentHighlight = target;
            currentHighlight?.Highlight();
        }

        public void ClearHighlight()
        {
            currentHighlight?.Unhighlight();
            currentHighlight = null;
        }

        void OnDisable() => ClearHighlight();
    }
}