using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CombatState : ICharacterState
    {
        public void Enter(Character character) => Debug.Log($"{character.name} entered combat!");
        public void Update(Character character) { }
        public void Exit(Character character) { }
    }
}