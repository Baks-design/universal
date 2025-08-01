namespace Universal.Runtime.Behaviours.Characters
{
    public interface ICharacterServices
    {
        bool ContainsCharacter(CharacterSettings settings);
        void AddCharacterToRoster();
        void RemoveCharacterFromRoster(int index);
        void NextCharacter();
        void PreviousCharacter();
        int GetCurrentCharacterIndex();
    }
}