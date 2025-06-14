using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterRotation
    {
        readonly CharacterMovementController controller;
        readonly CharacterController character;
        readonly CharacterData data;

        public CharacterRotation(
            CharacterMovementController controller,
            CharacterController character,
            CharacterData data)
        {
            this.controller = controller;
            this.character = character;
            this.data = data;
        }
    }
}