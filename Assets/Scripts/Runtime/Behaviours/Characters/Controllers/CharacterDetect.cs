using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterDetect : MonoBehaviour
    {
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float raySphereRadius = 0.3f;
        [SerializeField] float raySphereMaxDistance = 2f;
        bool isColl;
        RaycastHit hitInfo;
        Camera mainCamera;
        ICharacterServices characterServices;

        void Start()
        {
            mainCamera = Camera.main;
            ServiceLocator.Global.Get(out characterServices);
        }

        public void DetectBodies(IInvestigateInputReader investigateInput)
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

        public void OnAddCharacter()
        {
            if (!isColl) return;

            if (hitInfo.collider.TryGetComponent(out CharacterPlayerController character))
            {
                if (!characterServices.ContainsCharacter(character.CharacterData))
                {
                    characterServices.AddCharacterToRoster(character.CharacterData);
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
    }
}