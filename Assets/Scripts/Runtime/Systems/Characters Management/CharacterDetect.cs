using UnityEngine;
using UnityEngine.InputSystem;
using KBCore.Refs;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Systems.CharactersManagement
{
    public class CharacterDetect : MonoBehaviour
    {
        [SerializeField, Self] CharacterManager characterManager;
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float raySphereRadius = 0.3f;
        [SerializeField] float raySphereMaxDistance = 20f;
        bool isColl;
        RaycastHit hitInfo;
        Camera mainCamera;

        void Start() => mainCamera = Camera.main;

        void OnEnable() => PlayerMapInputProvider.AddCharacter.started += OnAddCharacter;

        void OnDisable() => PlayerMapInputProvider.AddCharacter.started -= OnAddCharacter;

        void OnAddCharacter(InputAction.CallbackContext context)
        {
            if (!isColl) return;

            if (hitInfo.collider.TryGetComponent(out CharacterMovementController character))
            {
                if (!characterManager.ContainsCharacter(character.CharacterData))
                {
                    characterManager.AddCharacterToRoster(character.CharacterData);
                    RemoveCharacterFromScene(character.gameObject);
                }
            }
        }

        void RemoveCharacterFromScene(GameObject characterObj, bool byDestroy = false)
        {
            if (byDestroy)
                Destroy(characterObj);
            else
                characterObj.SetActive(false);
        }

        void Update() => DetectBodies();

        void DetectBodies()
        {
            var getRay = mainCamera.ScreenPointToRay(
                PlayerMapInputProvider.Look.ReadValue<Vector2>());
            isColl = Physics.SphereCast(
                getRay.origin,
                raySphereRadius,
                getRay.direction,
                out var _,
                raySphereMaxDistance,
                characterLayer
            );
        }
    }
}