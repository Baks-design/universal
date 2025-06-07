using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Universal.Runtime.Utilities.Tools;
using Universal.Runtime.Utilities.Helpers;
using UnityEngine.InputSystem;
using Universal.Runtime.Behaviours.Characters;
using Unity.Cinemachine;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterManager : PersistentSingleton<CharacterManager>
    {
        [SerializeField] CharacterData characterData;
        [SerializeField] CinemachineCamera cinemachine;
        [SerializeField] GameObject characterContainer;
        [SerializeField] GameObject[] spawnPoints;
        List<IPlayableCharacter> characterRoster = new(7);
        IPlayableCharacter currentCharacter;
        Vector3 lastActivePosition;
        Quaternion lastActiveRotation;
        const int maxCharacters = 7;
        readonly HashSet<CharacterData> rosterData = new();

        public bool ContainsCharacter(CharacterData data) => rosterData.Contains(data);
        public IReadOnlyList<IPlayableCharacter> CharacterRoster => characterRoster.AsReadOnly();

        void Start() => AddCharacterToRoster(characterData);

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMapInputProvider.SwitchCharacter.started += NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started += PreviousCharacter;
        }

        protected override void OnDisable()
        {
            PlayerMapInputProvider.SwitchCharacter.started -= NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started -= PreviousCharacter;
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            PlayerMapInputProvider.SwitchCharacter.started -= NextCharacter;
            PlayerMapInputProvider.SwitchCharacter.started -= PreviousCharacter;
            base.OnDestroy();
        }

        void NextCharacter(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() <= 0f || characterRoster.Count == 0) return;

            var currentIndex = characterRoster.IndexOf(currentCharacter);
            var nextIndex = (currentIndex + 1) % characterRoster.Count;
            SwitchCharacter(nextIndex);
        }

        void PreviousCharacter(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() >= 0f || characterRoster.Count == 0) return;

            var currentIndex = characterRoster.IndexOf(currentCharacter);
            var prevIndex = (currentIndex - 1 + characterRoster.Count) % characterRoster.Count;
            SwitchCharacter(prevIndex);
        }

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

            if (!charObj.TryGetComponent(out Character character))
            {
                Debug.LogError("Instantiated prefab doesn't have Character component!");
                return;
            }

            character.Initialize(characterData);

            if (characterRoster.Count == 0)
            {
                character.LastPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                character.LastRotation = Quaternion.identity;
            }
            else
            {
                character.LastPosition = lastActivePosition;
                character.LastRotation = lastActiveRotation;
            }

            character.transform.SetPositionAndRotation(character.LastPosition, character.LastRotation);

            rosterData.Add(characterData);
            characterRoster.Add(character);
            character.Deactivate();

            if (currentCharacter == null)
                SwitchCharacter(0);
        }

        public void RemoveCharacterFromRoster(int index)
        {
            if (index >= 0 && index < characterRoster.Count)
            {
                var character = characterRoster[index];
                rosterData.Remove(character.Data);

                if (currentCharacter == character)
                    SwitchCharacter(Mathf.Max(0, index - 1));

                if (character is MonoBehaviour mb)
                    Addressables.ReleaseInstance(mb.gameObject);

                characterRoster.RemoveAt(index);
            }
        }

        void SwitchCharacter(int index)
        {
            if (index < 0 || index >= characterRoster.Count) return;

            // Store current position before switching
            if (currentCharacter != null)
            {
                lastActivePosition = currentCharacter.CharacterTransform.position;
                lastActiveRotation = currentCharacter.CharacterTransform.rotation;
                currentCharacter.Deactivate();
            }

            currentCharacter = characterRoster[index];

            // Apply the stored position/rotation to the new character
            currentCharacter.LastPosition = lastActivePosition;
            currentCharacter.LastRotation = lastActiveRotation;

            currentCharacter.CharacterTransform.SetPositionAndRotation(
                lastActivePosition,
                lastActiveRotation
            );

            currentCharacter.Activate();

            if (cinemachine != null && cinemachine.Target.TrackingTarget != null)
                cinemachine.Target.TrackingTarget = currentCharacter.CharacterTransform;
            Debug.Log($"Switched to {currentCharacter.Data.characterName} at {currentCharacter.LastPosition}");
        }
    }
}