using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Systems.InventoryManagement;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class PickupController : MonoBehaviour //TODO: Adjust
    {
        [SerializeField] Inventory inventory;
        [SerializeField] LayerMask objectsLayers;
        [SerializeField] float interactionRadius = 0.3f;
        [SerializeField] float interactionRange = 1f;
        IInteractable interactable;
        IInvestigateInputReader input;
        Ray ray;
        Transform mainCamera;
        bool isHit;

        public Inventory Inventory { get => inventory; set => inventory = value; }
        public Vector3 GetAimDirection => ray.direction;

        void Awake()
        {
            mainCamera = Camera.main.transform;
            ServiceLocator.Global.Get(out input);
        }

        void OnEnable() => input.Interact += OnInteractStarted;

        void OnDisable() => input.Interact -= OnInteractStarted;

        void OnInteractStarted()
        {
            if (!isHit) return;
            interactable?.OnInteract(this);
        }

        void Update() => ProcessDetection();

        void ProcessDetection()
        {
            var ray = new Ray(mainCamera.localPosition, mainCamera.forward);
            isHit = Physics.SphereCast(
                ray.origin, interactionRadius, ray.direction, out var hit,
                interactionRange, objectsLayers, QueryTriggerInteraction.Ignore);
            if (isHit)
                hit.collider.TryGetComponent(out interactable);
        }

        void OnDrawGizmosSelected()
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