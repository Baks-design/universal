using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class Garcian : Character
    {
        readonly List<Character> deadCharacters = new();

        public override void UseAbility()
        {
            if (deadCharacters.Count <= 0)
                return;

            var revived = deadCharacters[0];
            revived.Activate();
            deadCharacters.Remove(revived);
            Debug.Log($"Garcian revived {revived.name}!");
        }

        public override void ProcessMovement(float deltaTime)
        => tr.Translate(3f * deltaTime * Vector3.forward);

        public void RegisterDeadCharacter(Character character) => deadCharacters.Add(character);
    }
}