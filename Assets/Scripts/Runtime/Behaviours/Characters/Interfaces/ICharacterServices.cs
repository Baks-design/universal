namespace Universal.Runtime.Behaviours.Characters
{
    public interface ICharacterServices
    {
        bool ContainsCharacter(CharacterData data);
        void AddCharacterToRoster(CharacterData characterData);
        void RemoveCharacterFromRoster(int index);
        void NextCharacter();
        void PreviousCharacter();
        int GetCurrentCharacterIndex();
    }
}