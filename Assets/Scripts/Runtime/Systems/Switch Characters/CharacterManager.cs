using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterManager : PersistentSingleton<CharacterManager> //FIXME: Spawn
    {
        [SerializeField, Self] CharacterSwitchVisual characterSwitchVisual;
        [SerializeField, InLineEditor] CharacterDataSO initDataSO;
        [SerializeField] GameObject[] spawnPoints;
        int currentIndex = 0;
        readonly List<CharacterDataSO> charactersStoraged = new(7);
        readonly List<Character> spawnedCharacters = new(7);

        public bool HasSlotsAvailable => charactersStoraged.Count < 7;
        public Character CurrentCharacter => spawnedCharacters.Count > 0 ? spawnedCharacters[currentIndex] : null;
        public int CurrentIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }

        void Start() => SpawnInitCharacter();

        void SpawnInitCharacter()
        {
            var randPoint = Random.Range(0, spawnPoints.Length);
            var newChar = Addressables.InstantiateAsync(
                initDataSO.prefab,
                null,
                spawnPoints[randPoint].transform
            ).WaitForCompletion();

            if (newChar.TryGetComponent(out Character character))
            {
                charactersStoraged.Add(initDataSO);
                spawnedCharacters.Add(character);
                character.Deactivate();
                SwitchTo(0);
            }
        }

        public void NextCharacter()
        {
            if (spawnedCharacters.Count == 0)
                return;

            SwitchTo((currentIndex + 1) % spawnedCharacters.Count);
        }

        public void SwitchTo(int index)
        {
            if (index < 0 || index >= spawnedCharacters.Count || CurrentCharacter == null)
                return;

            characterSwitchVisual.HandleChangedEffect(index, CurrentCharacter);
        }

        public void AddCharacter(CharacterDataSO item) => charactersStoraged.Add(item);

        public void RemoveCharacter(CharacterDataSO item) => charactersStoraged.Remove(item);

        public void ClearCharacter(CharacterDataSO item) => charactersStoraged.Clear();
    }
}