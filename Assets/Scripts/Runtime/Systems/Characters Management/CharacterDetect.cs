using UnityEngine;
using KBCore.Refs;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

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
        IInvestigateInputReader investigateInput;

        void Start()
        {
            ServiceLocator.Global.Get(out investigateInput);
            investigateInput.AddCharacter += OnAddCharacter;

            mainCamera = Camera.main;
        }

        void OnDisable() => investigateInput.AddCharacter -= OnAddCharacter;

        void OnAddCharacter()
        {
            if (!isColl) return;

            if (hitInfo.collider.TryGetComponent(out PlayerController character))
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
            Vector3 mousPos = default;
            mousPos.x = investigateInput.LookDirection.x;
            mousPos.z = investigateInput.LookDirection.y;
            var getRay = mainCamera.ScreenPointToRay(mousPos);
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