using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class PickupInteractable : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;
        public string InteractionPrompt => "";

        public void OnInteract(PickupController interactor)
        {
            Destroy(gameObject);
        }
    }
}