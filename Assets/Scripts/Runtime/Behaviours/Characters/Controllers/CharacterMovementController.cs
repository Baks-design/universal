using UnityEngine;
using KBCore.Refs;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField, Child] PlayerController controller;
        [SerializeField] CharacterData data;
        IMovementInputReader movementInput;

        public CharacterRotation CharacterRotation { get; private set; }
        public CharacterMovement CharacterMovement { get; private set; }

        void OnEnable()
        {
            ServiceLocator.Global.Get(out movementInput);
            
            CharacterRotation = new CharacterRotation(this, transform, data);
            CharacterMovement = new CharacterMovement(controller, transform, data, Camera.main, movementInput);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            CharacterMovement.DrawMovementGizmos();
        }
#endif
    }
}