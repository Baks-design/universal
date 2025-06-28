using UnityEngine;
using Universal.Runtime.Systems.InventoryManagement;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class PickupController : MonoBehaviour
    {
        [SerializeField] Inventory inventory;
        [SerializeField] LayerMask objectsLayers;
        [SerializeField] float interactionRadius = 0.3f;
        [SerializeField] float interactionRange = 1f;
        RaycastHit[] interactionHits;
        IInteractable interactable;
        Ray ray;
        Camera mainCamera;
        int hitCount;

        public Inventory Inventory { get => inventory; set => inventory = value; }
        public Vector3 GetAimDirection => ray.direction;

        void Awake()
        {
            interactionHits = new RaycastHit[10];
            mainCamera = Camera.main;
        }

        void Update() => ProcessDetection();

        void ProcessDetection()
        {
            var center = new Vector3(0.5f, 0.5f, 0f);
            ray = mainCamera.ViewportPointToRay(center);

            hitCount = Physics.SphereCastNonAlloc(
                ray.origin, interactionRadius, ray.direction,
                interactionHits, interactionRange, objectsLayers
            );

            for (var i = 0; i < hitCount; i++)
                interactionHits[i].collider.TryGetComponent(out interactable);
            if (hitCount == 0)
                interactable = null;
        }

        public void OnInteractStarted()
        {
            if (hitCount == 0 || interactable == null) return;
            interactable.OnInteract(this);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw interaction range
            Gizmos.color = interactable != null ? Color.yellow : Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * interactionRange);

            // Draw sphere cast area
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            var sphereEnd = ray.origin + ray.direction * interactionRange;
            Gizmos.DrawWireSphere(ray.origin, interactionRadius);
            Gizmos.DrawWireSphere(sphereEnd, interactionRadius);
        }
    }
}