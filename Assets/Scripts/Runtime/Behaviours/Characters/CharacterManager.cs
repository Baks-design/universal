using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Collections;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Helpers;
using Alchemy.Inspector;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterManager : MonoBehaviour, ICharacterServices
    {
        [SerializeField, InlineEditor] CharacterSettings characterData;
        const int maxCharacters = 7;
        readonly List<KeyValuePair<IPlayableCharacter, IEnableComponent>> characterRoster = new(maxCharacters);
        readonly HashSet<CharacterSettings> rosterData = new(maxCharacters);
        Coroutine activeCoroutine;
        IPlayableCharacter currentCharacter;
        IEnableComponent enableComponent;
        IInputReaderServices inputServices;
        ISpawnServices spawnServices;

        public Vector3 LastActivePosition { get; set; }
        public Quaternion LastActiveRotation { get; set; }

        void Awake() => ServiceLocator.Global.Register<ICharacterServices>(this);

        void Start()
        {
            ServiceLocator.Global.Get(out spawnServices);
            ServiceLocator.Global.Get(out inputServices);
        }

        public void NextCharacter()
        {
            if (characterRoster.Count == 0) return;

            var currentIndex = GetCurrentCharacterIndex();
            if (currentIndex == -1) return;

            var nextIndex = (currentIndex + 1) % characterRoster.Count;
            SwitchCharacter(nextIndex);
        }

        public void PreviousCharacter()
        {
            if (characterRoster.Count == 0) return;

            var currentIndex = GetCurrentCharacterIndex();
            if (currentIndex == -1) return;

            var prevIndex = (currentIndex - 1 + characterRoster.Count) % characterRoster.Count;
            SwitchCharacter(prevIndex);
        }

        public int GetCurrentCharacterIndex()
        => characterRoster.FindIndex(kvp => kvp.Key == currentCharacter || kvp.Value == enableComponent);

        public bool ContainsCharacter(CharacterSettings settings) => rosterData.Contains(settings);

        public void AddCharacterToRoster()
        {
            inputServices.ChangeToMovementMap();

            if (characterRoster.Count >= maxCharacters || rosterData.Contains(characterData)) return;

            var charObj = Addressables.InstantiateAsync(characterData.characterPrefab, transform).WaitForCompletion();
            if (!charObj.TryGetComponent(out IPlayableCharacter character) ||
                !charObj.TryGetComponent(out IEnableComponent enableComp))
            {
                Addressables.ReleaseInstance(charObj);
                return;
            }

            // Set positions BEFORE initialization
            if (characterRoster.Count == 0)
            {
                var spawnPoint = spawnServices.GetSpawnPoint(); //FIXME
                character.LastPosition = spawnPoint.Position;
                character.LastRotation = spawnPoint.Rotation;
            }
            else
            {
                character.LastPosition = LastActivePosition;
                character.LastRotation = LastActiveRotation;
            }

            // Now initialize with the positions set
            character.Initialize(characterData);

            // Update active position tracking
            if (characterRoster.Count == 0)
            {
                LastActivePosition = character.LastPosition;
                LastActiveRotation = character.LastRotation;
            }

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
            rosterData.Remove(character.Key.Settings);

            if (currentCharacter == character.Key)
            {
                var newIndex = Clamp(index - 1, 0, characterRoster.Count - 2);
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
            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);
            activeCoroutine = StartCoroutine(SwitchCharacterRoutine(newCharacter.Key, newCharacter.Value));
        }

        IEnumerator SwitchCharacterRoutine(IPlayableCharacter newCharacter, IEnableComponent newEnableComponent)
        {
            // Save current character's position before switching
            if (currentCharacter != null)
            {
                LastActivePosition = currentCharacter.CharacterTransform.localPosition;
                LastActiveRotation = currentCharacter.CharacterTransform.localRotation;
                currentCharacter.LastPosition = LastActivePosition;
                currentCharacter.LastRotation = LastActiveRotation;
                enableComponent.Deactivate();
            }

            // Update references
            currentCharacter = newCharacter;
            enableComponent = newEnableComponent;

            enableComponent.Activate();
            yield return null;
        }
    }
}