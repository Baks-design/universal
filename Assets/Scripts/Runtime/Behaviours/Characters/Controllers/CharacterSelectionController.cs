using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSelectionController : MonoBehaviour
    {
        [SerializeField, Parent] CharacterCollisionController collision;
        ICharacterServices services;
        IInvestigateInputReader input;

        void Awake()
        {
            ServiceLocator.Global.Get(out services);
            ServiceLocator.Global.Get(out input);
        }

        void OnEnable() => input.AddCharacter += OnAddCharacter;

        void OnDisable() => input.AddCharacter -= OnAddCharacter;

        void OnAddCharacter()
        {
            if (!collision.HasObstacle) return;

            if (collision.ObstacleHit.collider.TryGetComponent(out CharacterPlayerController character))
            {
                if (!services.ContainsCharacter(character.Settings))
                {
                    services.AddCharacterToRoster(character.Settings);
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