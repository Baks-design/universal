namespace Universal.Runtime.Systems.InteractionObjects
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        bool IsInteractable { get; }

        void OnInteract(PickupController interactor);
    }
}