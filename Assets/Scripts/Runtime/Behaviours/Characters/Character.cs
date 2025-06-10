using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class Character : MonoBehaviour, IEnableComponent, IPlayableCharacter
    {
        [SerializeField, InLineEditor] CharacterData characterData;

        public CharacterData CharacterData => characterData;
        public Transform CharacterTransform => transform;
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }

        public void Initialize(CharacterData data) => characterData = data;

        public virtual void Activate()
        {
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(LastPosition, LastRotation);
        }

        public virtual void Deactivate()
        {
            LastPosition = transform.position;
            LastRotation = transform.rotation;
            gameObject.SetActive(false);
        }
    }
}