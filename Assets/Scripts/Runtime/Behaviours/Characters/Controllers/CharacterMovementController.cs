using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField, Self] CharacterController controller;
        [SerializeField, InLineEditor] CharacterData data;

        public CharacterMovement CharacterMovement { get; set; }

        void Awake() => CharacterMovement = new CharacterMovement(transform, controller, data);
    }
}
