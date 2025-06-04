using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class AbilityDataSO : ScriptableObject
    {
        public string abilityName;
        public Sprite icon;
        public float cooldown;

        public abstract void UseAbility(Character character);
    }
}