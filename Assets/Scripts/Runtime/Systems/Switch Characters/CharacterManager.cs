using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Universal.Runtime.Behaviours.Characters;
using System.Collections;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterManager : MonoBehaviour, ICharacterServices
    {
        [SerializeField] GameObject characterContainer;
        [SerializeField] GameObject[] spawnPoints;
        [SerializeField, InLineEditor] CharacterData characterData;
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

        void OnEnable()
        {
            PlayerMapInputProvider.SwitchCharacter.started += NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started += PreviousCharacter;
        }

        void OnDisable()
        {
            PlayerMapInputProvider.SwitchCharacter.started -= NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started -= PreviousCharacter;
        }

        void OnDestroy()
        {
            PlayerMapInputProvider.SwitchCharacter.started -= NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started -= PreviousCharacter;
        }

        void NextCharacter(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() <= 0f || characterRoster.Count == 0) return;

            var currentIndex = GetCurrentCharacterIndex();
            if (currentIndex == -1) return;

            var nextIndex = (currentIndex + 1) % characterRoster.Count;
            SwitchCharacter(nextIndex);
        }

        void PreviousCharacter(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() >= 0f || characterRoster.Count == 0) return;

            var currentIndex = GetCurrentCharacterIndex();
            if (currentIndex == -1) return;

            var prevIndex = (currentIndex - 1 + characterRoster.Count) % characterRoster.Count;
            SwitchCharacter(prevIndex);
        }

        int GetCurrentCharacterIndex()
        => characterRoster.FindIndex(kvp => kvp.Key == currentCharacter || kvp.Value == enableComponent);

        public bool ContainsCharacter(CharacterData data) => rosterData.Contains(data);

        public void AddCharacterToRoster(CharacterData characterData)
        {
            if (characterRoster.Count >= maxCharacters)
            {
                Debug.LogWarning("Character roster is full!");
                return;
            }
            if (rosterData.Contains(characterData))
            {
                Debug.LogWarning($"Character {characterData.characterName} already in roster!");
                return;
            }

            var charObj = Addressables.InstantiateAsync(
                characterData.characterPrefab,
                characterContainer.transform
            ).WaitForCompletion();
            if (!charObj.TryGetComponent(out IPlayableCharacter character))
            {
                Debug.LogError("Instantiated prefab doesn't implement IPlayableCharacter!");
                Addressables.ReleaseInstance(charObj);
                return;
            }
            if (!charObj.TryGetComponent(out IEnableComponent enableComp))
            {
                Debug.LogError("Instantiated prefab doesn't implement IEnableComponent!");
                Addressables.ReleaseInstance(charObj);
                return;
            }

            character.Initialize(characterData);

            if (characterRoster.Count == 0)
            {
                // Set spawn position for first character
                var randomPoint = Random.Range(0, spawnPoints.Length);
                var spawnPoint = spawnPoints[randomPoint].transform;
                character.CharacterTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                lastActivePosition = spawnPoint.position;
                lastActiveRotation = spawnPoint.rotation;
            }
            else
                // Use last active position for subsequent characters
                character.CharacterTransform.SetPositionAndRotation(lastActivePosition, lastActiveRotation);

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
                lastActivePosition = currentCharacter.CharacterTransform.position;
                lastActiveRotation = currentCharacter.CharacterTransform.rotation;
                enableComponent.Deactivate();
            }

            // Update references
            currentCharacter = newCharacter;
            enableComponent = newEnableComponent;

            // Apply the stored position/rotation to the new character
            currentCharacter.CharacterTransform.SetPositionAndRotation(lastActivePosition, lastActiveRotation);

            enableComponent.Activate();
            //Debug.Log($"Switched to {currentCharacter.CharacterData.characterName}");

            yield return null;
        }
    }
}