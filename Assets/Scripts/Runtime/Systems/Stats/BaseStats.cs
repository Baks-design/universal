using UnityEngine;

namespace Universal.Runtime.Systems.Stats
{
    [CreateAssetMenu(menuName = "Data/Stats/BaseStats")]
    public class BaseStats : ScriptableObject
    {
        public int attack = 10;
        public int defense = 20;
    }
}