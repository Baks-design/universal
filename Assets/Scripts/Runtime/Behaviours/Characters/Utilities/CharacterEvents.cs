using System;

namespace Universal.Runtime.Behaviours.Characters
{
    public static class CharacterEvents
    {
        public static event Action<Character> OnCharacterSwitched;

        public static void RaiseCharacterSwitched(Character character) => OnCharacterSwitched?.Invoke(character);
    }
}