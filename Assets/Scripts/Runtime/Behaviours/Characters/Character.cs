using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class Character : MonoBehaviour, IPlayableCharacter
    {
        public abstract string CharacterName { get; }

        public virtual void Activate()
        {
            gameObject.SetActive(true);
            // Add character-specific activation logic
        }

        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
            // Add character-specific deactivation logic
        }
    }
}