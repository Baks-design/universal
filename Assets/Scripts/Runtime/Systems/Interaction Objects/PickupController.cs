using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class PickupController : MonoBehaviour
    {
        [SerializeField] PickupSettings settings;
        IInteractable interactable;
        IInvestigateInputReader input;
        Ray ray;
        Transform mainCamera;
        bool isHit;

        public Vector3 GetAimDirection => ray.direction;

        void Awake()
        {
            mainCamera = Camera.main.transform;
            ServiceLocator.Global.Get(out input);
        }

        void OnEnable() => input.Interact += OnInteract;

        void OnDisable() => input.Interact -= OnInteract;

        void OnInteract()
        {
            if (interactable == null) return;

            interactable.OnInteract(this);
            Debug.Log(true);
        }

        void Update() => ProcessDetection();

        void ProcessDetection()
        {
            var ray = new Ray(mainCamera.localPosition, mainCamera.forward);

            isHit = Physics.SphereCast(
                ray.origin, settings.interactionRadius, ray.direction, out var hit,
               settings.interactionRange, settings.objectsLayers, QueryTriggerInteraction.Ignore);

            if (isHit)
                hit.collider.TryGetComponent(out interactable);
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            // Draw interaction range
            Gizmos.color = isHit ? Color.green : Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * settings.interactionRange);

            // Draw sphere cast area
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            var sphereEnd = ray.origin + ray.direction * settings.interactionRange;
            Gizmos.DrawWireSphere(ray.origin, settings.interactionRadius);
            Gizmos.DrawWireSphere(sphereEnd, settings.interactionRadius);
        }
    }
}