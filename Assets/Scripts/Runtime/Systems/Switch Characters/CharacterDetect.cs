using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using UnityEngine.InputSystem;
using KBCore.Refs;
using Universal.Runtime.Systems.ManagedUpdate;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterDetect : AManagedBehaviour, IUpdatable
    {
        [SerializeField, Self] CharacterManager characterManager;
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float checkradius = 0.3f;
        [SerializeField] float maxDistance = 20f;
        bool isColl;
        RaycastHit hitInfo;
        Camera mainCamera;

        void Start() => mainCamera = Camera.main;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMapInputProvider.AddCharacter.started += OnAddCharacter;
        }

        protected override void OnDisable()
        {
            PlayerMapInputProvider.AddCharacter.started -= OnAddCharacter;
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            PlayerMapInputProvider.AddCharacter.started -= OnAddCharacter;
            base.OnDestroy();
        }

        void OnAddCharacter(InputAction.CallbackContext context)
        {
            if (isColl &&
                hitInfo.collider.TryGetComponent(out IPlayableCharacter character) &&
                !characterManager.CharacterRoster.Contains(character))
            {
                characterManager.AddCharacterToRoster(character);
                Debug.Log($"Added {character.CharacterName} to roster");

                var characterDetected = hitInfo.collider.transform;
                var characterContainer = characterManager.CharacterContainer.transform;
                characterDetected.SetParent(characterContainer);
            }
        }

        void IUpdatable.ManagedUpdate(float deltaTime, float time) => DetectBodies();

        void DetectBodies()
        {
            var getRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            (isColl, hitInfo) = GamePhysics.SphereCast(
                getRay.origin, getRay.direction, checkradius, 0f, maxDistance, characterLayer);
        }
    }
}