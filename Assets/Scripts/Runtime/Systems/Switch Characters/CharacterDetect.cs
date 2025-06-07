using UnityEngine;
using Universal.Runtime.Utilities.Helpers;
using UnityEngine.InputSystem;
using KBCore.Refs;
using Universal.Runtime.Systems.ManagedUpdate;
using Universal.Runtime.Behaviours.Characters;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterDetect : AManagedBehaviour, IUpdatable
    {
        [SerializeField, Self] CharacterManager characterManager;
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float raySphereRadius = 0.3f;
        [SerializeField] float raySphereMaxDistance = 20f;
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
            if (!isColl) return;

            if (hitInfo.collider.TryGetComponent(out Character character))
            {
                if (!characterManager.ContainsCharacter(character.Data))
                {
                    characterManager.AddCharacterToRoster(character.Data);
                    Debug.Log($"Added {character.Data.characterName} to roster");

                    RemoveCharacterFromScene(character.gameObject);
                }
                else
                    Debug.Log($"{character.Data.characterName} already in roster");
            }
        }

        void RemoveCharacterFromScene(GameObject characterObj, bool byDestroy = false)
        {
            if (byDestroy)
                Destroy(characterObj);
            else
                characterObj.SetActive(false);
        }

        void IUpdatable.ManagedUpdate(float deltaTime, float time) => DetectBodies();

        void DetectBodies()
        {
            var getRay = mainCamera.ScreenPointToRay(PlayerMapInputProvider.MousePos);
            (isColl, hitInfo) = GamePhysics.SphereCast(
                getRay.origin,
                getRay.direction,
                raySphereRadius,
                0f,
                raySphereMaxDistance,
                characterLayer
            );
        }
    }
}