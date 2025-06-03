namespace Universal.Runtime.Behaviours.Characters
{
    public interface ICharacterState
    {
        void Enter(Character character);
        void Update(Character character);
        void Exit(Character character);
    }
}