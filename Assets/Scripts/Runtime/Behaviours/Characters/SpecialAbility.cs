using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class SpecialAbility
    {
        public string abilityName;
        public float cooldown;
        public Sprite abilityIcon;
    }
}