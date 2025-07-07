namespace Universal.Runtime.Systems.TargetingSelection
{
    public interface IHighlightController
    {
        void SetHighlight(IHighlightable target);
        void ClearHighlight();
    }
}