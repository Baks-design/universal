using Universal.Runtime.Systems.Stats;

namespace Universal.Runtime.Behaviours
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data/Stats/PickupData")]
    public class PickupData : ScriptableObject
    {
        public StatType type = StatType.Attack;
        public OperatorType operatorType = OperatorType.Add;
        public int value = 10;
        public float duration = 5f;
    }
}