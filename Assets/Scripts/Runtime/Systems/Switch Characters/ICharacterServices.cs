using Universal.Runtime.Behaviours.Characters;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public interface ICharacterServices
    {
        bool ContainsCharacter(CharacterData data);
        void AddCharacterToRoster(CharacterData characterData);
        void RemoveCharacterFromRoster(int index);
    }
}