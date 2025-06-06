namespace Universal.Runtime.Behaviours.Characters
{
    public interface IPlayableCharacter
    {
        string CharacterName { get; }

        void Activate();
        void Deactivate();
    }
}