using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class Garcian : Character
    {
        readonly List<Character> deadCharacters = new();

        public override void Activate() => SetState(new IdleState());

        public override void Deactivate() => SetState(new DisabledState());

        public override void UseAbility()
        {
            if (deadCharacters.Count <= 0)
                return;

            var revived = deadCharacters[0];
            revived.Activate();
            deadCharacters.Remove(revived);
            Debug.Log($"Garcian revived {revived.name}!");
        }

        public override void Move() => transform.Translate(3f * Time.deltaTime * Vector3.forward);

        public void RegisterDeadCharacter(Character character) => deadCharacters.Add(character);
    }
}