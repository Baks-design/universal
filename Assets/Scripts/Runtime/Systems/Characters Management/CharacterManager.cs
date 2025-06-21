using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Universal.Runtime.Behaviours.Characters;
using System.Collections;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Alchemy.Inspector;

namespace Universal.Runtime.Systems.CharactersManagement
{
    public class CharacterManager : MonoBehaviour, ICharacterServices //TODO: Adjust Character Manager
    {
        [SerializeField] Grid grid;
        [SerializeField] GameObject characterContainer;
        [SerializeField] GameObject[] spawnPoints;
        [SerializeField, InlineEditor] CharacterData characterData;
        IPlayableCharacter currentCharacter;
        IEnableComponent enableComponent;
        Vector3 lastActivePosition;
        Quaternion lastActiveRotation;
        const int maxCharacters = 7;
        readonly List<KeyValuePair<IPlayableCharacter, IEnableComponent>> characterRoster = new(maxCharacters);
        readonly HashSet<CharacterData> rosterData = new(maxCharacters);

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<ICharacterServices>(this);
        }

        void Start() => AddCharacterToRoster(characterData);

        void OnEnable() => PlayerMapInputProvider.SwitchCharacter.started += NextCharacter;

        void OnDisable() => PlayerMapInputProvider.SwitchCharacter.started -= NextCharacter;

        void NextCharacter(InputAction.CallbackContext context)
        {
            if (characterRoster.Count == 0) return;

            var currentIndex = GetCurrentCharacterIndex();
            if (currentIndex == -1) return;

            switch (context.ReadValue<float>())
            {
                case > 0f:
                    var nextIndex = (currentIndex + 1) % characterRoster.Count;
                    SwitchCharacter(nextIndex);
                    break;
                case < 0f:
                    var prevIndex = (currentIndex - 1 + characterRoster.Count) % characterRoster.Count;
                    SwitchCharacter(prevIndex);
                    break;
            }
        }

        int GetCurrentCharacterIndex()
        => characterRoster.FindIndex(kvp => kvp.Key == currentCharacter || kvp.Value == enableComponent);

        public bool ContainsCharacter(CharacterData data) => rosterData.Contains(data);

        public void AddCharacterToRoster(CharacterData characterData)
        {
            if (characterRoster.Count >= maxCharacters ||
                rosterData.Contains(characterData)) return;

            var charObj = Addressables.InstantiateAsync(
                characterData.characterPrefab,
                characterContainer.transform
            ).WaitForCompletion();
            if (!charObj.TryGetComponent(out IPlayableCharacter character))
            {
                Addressables.ReleaseInstance(charObj);
                return;
            }
            if (!charObj.TryGetComponent(out IEnableComponent enableComp))
            {
                Addressables.ReleaseInstance(charObj);
                return;
            }

            character.Initialize(characterData, grid);

            if (characterRoster.Count == 0)
            {
                // Set spawn position for first character
                var randomPoint = Random.Range(0, spawnPoints.Length);
                var spawnPoint = spawnPoints[randomPoint].transform;
                character.CharacterTransform.SetLocalPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                lastActivePosition = spawnPoint.position;
                lastActiveRotation = spawnPoint.rotation;
            }
            else
                // Use last active position for subsequent characters
                character.CharacterTransform.SetLocalPositionAndRotation(lastActivePosition, lastActiveRotation);

            rosterData.Add(characterData);
            characterRoster.Add(new KeyValuePair<IPlayableCharacter, IEnableComponent>(character, enableComp));
            enableComp.Deactivate();

            if (currentCharacter == null)
                SwitchCharacter(0);
        }

        public void RemoveCharacterFromRoster(int index)
        {
            if (index < 0 || index >= characterRoster.Count) return;

            var character = characterRoster[index];
            rosterData.Remove(character.Key.CharacterData);

            if (currentCharacter == character.Key)
            {
                var newIndex = Mathf.Clamp(index - 1, 0, characterRoster.Count - 2);
                SwitchCharacter(newIndex);
            }

            if (character.Key is MonoBehaviour mb)
                Addressables.ReleaseInstance(mb.gameObject);

            characterRoster.RemoveAt(index);
        }

        void SwitchCharacter(int index)
        {
            if (index < 0 || index >= characterRoster.Count) return;

            var newCharacter = characterRoster[index];
            StartCoroutine(SwitchCharacterRoutine(newCharacter.Key, newCharacter.Value));
        }

        IEnumerator SwitchCharacterRoutine(IPlayableCharacter newCharacter, IEnableComponent newEnableComponent)
        {
            // Store current position before switching
            if (currentCharacter != null)
            {
                lastActivePosition = currentCharacter.CharacterTransform.localPosition;
                lastActiveRotation = currentCharacter.CharacterTransform.localRotation;
                enableComponent.Deactivate();
            }

            // Update references
            currentCharacter = newCharacter;
            enableComponent = newEnableComponent;

            // Apply the stored position/rotation to the new character
            currentCharacter.CharacterTransform.SetLocalPositionAndRotation(lastActivePosition, lastActiveRotation);

            enableComponent.Activate();

            yield return null;
        }
    }
}