using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class IdleState : ICharacterState
    {
        public void Enter(Character character) => Debug.Log($"{character.name} is idle.");
        public void Update(Character character) { }
        public void Exit(Character character) { }
    }
}