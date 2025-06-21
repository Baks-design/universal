using UnityEngine;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class PickupInteractable : MonoBehaviour, IInteractable //TODO: Adjust Pickup
    {
        [SerializeField] Item item;

        public bool IsInteractable => true;
        public string InteractionPrompt => "";

        public void OnInteract(Interactor interactor)
        {
            interactor.Inventory.Controller.AddItems(item);
            Destroy(gameObject);
        }
    }
}