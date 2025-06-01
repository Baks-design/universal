using UnityEngine;

namespace Universal.Runtime.Systems.Tendency
{
    public class PlayerTendencyActions : MonoBehaviour
    {
        [SerializeField] int currentWorldIndex;
        WorldTendency tendencySystem;

        void Start() => tendencySystem = TendencyManager.Instance.GetTendencySystem;

        public void OnEnemyDefeated(bool isBoss = false)
        => tendencySystem.AdjustTendency(currentWorldIndex, isBoss ? 0.5f : 0.1f);

        public void OnPlayerDeath() => tendencySystem.AdjustTendency(currentWorldIndex, -0.2f);
    }
}