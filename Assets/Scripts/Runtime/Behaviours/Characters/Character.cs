using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class Character : MonoBehaviour, IPlayableCharacter
    {
        [SerializeField, InLineEditor] CharacterData characterData;

        public CharacterData Data => characterData;
        public Transform CharacterTransform => transform;
        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }

        // public Transform CharacterTransform => transform;
        // Transform IPlayableCharacter.CharacterTransform => this.transform;
        // public Vector3 LastPosition { get; set; }
        // public Quaternion LastRotation { get; set; }
        // public CharacterData Data => characterData;

        public void Initialize(CharacterData data) => characterData = data;

        public virtual void Activate()
        {
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(LastPosition, LastRotation);

            // Play spawn effects //TODO
            // if (characterData.spawnSound != null)
            //     AudioSource.PlayClipAtPoint(characterData.spawnSound, LastPosition);
            // if (characterData.spawnVFX != null)
            //     Instantiate(characterData.spawnVFX, LastPosition, LastRotation);
        }

        public virtual void Deactivate()
        {
            LastPosition = transform.position;
            LastRotation = transform.rotation;
            gameObject.SetActive(false);
        }
    }
}