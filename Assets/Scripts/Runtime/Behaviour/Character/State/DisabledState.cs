using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class DisabledState : ICharacterState
    {
        public void Enter(Character character)
        {
            character.TryGetComponent<Renderer>(out var rend);
            rend.sharedMaterial.color = Color.gray;
            Debug.Log($"{character.name} is disabled!");
        }

        public void Update(Character character) { }

        public void Exit(Character character)
        {
            character.TryGetComponent<Renderer>(out var rend);
            rend.sharedMaterial.color = Color.white;
        }
    }
}