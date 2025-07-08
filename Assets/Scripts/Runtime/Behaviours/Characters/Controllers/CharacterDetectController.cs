using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterDetectController : MonoBehaviour
    {
        [SerializeField, Parent] CharacterMovementController controller;
        [SerializeField, InlineEditor] CharacterData data;
        RaycastHit hitInfo;
        ICharacterServices characterServices;

        void Awake() => ServiceLocator.Global.Get(out characterServices);

        public void OnAddCharacter()
        {
            if (!controller.CollisionChecker.HasCharacter) return;

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