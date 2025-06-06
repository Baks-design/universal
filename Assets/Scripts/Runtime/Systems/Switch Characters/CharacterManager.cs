using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Universal.Runtime.Utilities.Tools;
using Universal.Runtime.Utilities.Helpers;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterManager : PersistentSingleton<CharacterManager>
    {
        [SerializeField] AssetReference defaultCharacterPrefab;
        [SerializeField] GameObject characterContainer;
        [SerializeField] GameObject[] spawnPoints;
        List<IPlayableCharacter> characterRoster = new();
        IPlayableCharacter currentCharacter;
        const int maxCharacters = 7;

        public List<IPlayableCharacter> CharacterRoster
        {
            get => characterRoster;
            set => characterRoster = value;
        }
        public GameObject CharacterContainer
        {
            get => characterContainer;
            set => characterContainer = value;
        }

        void Start()
        {
            var randPoint = Random.Range(0, spawnPoints.Length);

            var newChar = Addressables
                .InstantiateAsync(
                    defaultCharacterPrefab,
                    spawnPoints[randPoint].transform.position,
                    Quaternion.identity,
                    characterContainer.transform
                )
                .WaitForCompletion();

            if (newChar.TryGetComponent(out IPlayableCharacter IPlayableCharacter))
            {
                AddCharacterToRoster(IPlayableCharacter);
                SwitchCharacter(0);
            }
        }

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

        public void AddCharacterToRoster(IPlayableCharacter character)
        {
            if (characterRoster.Count >= maxCharacters)
            {
                Debug.LogWarning("Character roster is full!");
                return;
            }
            characterRoster.Add(character);
            character.Deactivate(); // New characters start inactive
        }

        public void RemoveCharacterFromRoster(int index)
        {
            if (index < 0 || index >= characterRoster.Count) return;

            // If removing current character, switch to default first
            if (currentCharacter == characterRoster[index])
                SwitchCharacter(0);

            // Proper cleanup (could use object pooling in a real scenario)
            if (characterRoster[index] is MonoBehaviour mb)
                Destroy(mb.gameObject);

            characterRoster.RemoveAt(index);
        }

        void SwitchCharacter(int index)
        {
            if (index < 0 || index >= characterRoster.Count) return;

            currentCharacter?.Deactivate();
            currentCharacter = characterRoster[index];
            currentCharacter.Activate();

            Debug.Log($"Switched to {currentCharacter.CharacterName}");
        }
    }
}