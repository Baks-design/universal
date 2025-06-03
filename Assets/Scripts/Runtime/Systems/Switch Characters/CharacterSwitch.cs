using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Systems.ManagedUpdate;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterSwitch : AManagedBehaviour, IUpdatable
    {
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float checkradius = 0.3f;
        [SerializeField] float maxDistance = 2f;
        Character currentPotential;
        Camera mainCamera;

        void Start() => mainCamera = Camera.main;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMapInputProvider.SwitchCharacter.started += OnSwitchCharacter;
            PlayerMapInputProvider.UseAbility.started += OnUseAbility;
        }

        void OnDestroy()
        {
            PlayerMapInputProvider.SwitchCharacter.started -= OnSwitchCharacter;
            PlayerMapInputProvider.UseAbility.started -= OnUseAbility;
        }

        void OnSwitchCharacter(InputAction.CallbackContext context)
        => CharacterManager.Instance.NextCharacter();

        void OnUseAbility(InputAction.CallbackContext context)
        {
            if (CharacterManager.Instance.CurrentCharacter == null)
                return;

            CharacterManager.Instance.CurrentCharacter.UseAbility();
        }

        void IUpdatable.ManagedUpdate(float deltaTime) => DetectBodies();

        void DetectBodies()
        {
            if (!CharacterManager.Instance.HasSlotsAvailable) return;

            var getRay = GetMouseRay();
            var (isColl, hitInfo) = GamePhysics.SphereCast(
              getRay.origin, getRay.direction, checkradius, 0f, maxDistance, characterLayer);

            if (isColl)
                if (hitInfo.collider.TryGetComponent(out currentPotential))
                    CharacterManager.Instance.AddCharacter(currentPotential.Data);
        }

        static Ray GetMouseRay()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return new Ray();
            var mouseScreenPos = Mouse.current.position.ReadValue();
            return mainCamera.ScreenPointToRay(mouseScreenPos);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw the sphere cast
            Gizmos.color = currentPotential != null ? Color.green : Color.red;
            var endPosition = mainCamera.transform.position + mainCamera.transform.forward * maxDistance;
            Gizmos.DrawLine(mainCamera.transform.position, endPosition);
            Gizmos.DrawWireSphere(mainCamera.transform.position, checkradius);
            Gizmos.DrawWireSphere(endPosition, checkradius);

            // Draw a small indicator at the current potential character if one exists
            if (currentPotential != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(currentPotential.transform.position, 0.2f);
            }
        }
    }
}