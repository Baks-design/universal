using UnityEngine;
using Universal.Runtime.Components.Collider;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterCollisionController : MonoBehaviour
    {
        [SerializeField, InLineEditor] CharacterCollisionData data;

        public CharacterGroundChecker GroundChecker { get; private set; }

        void Awake() => GroundChecker = new CharacterGroundChecker(data, transform);

        void Update() => GroundChecker.CheckGrounded();
    }
}