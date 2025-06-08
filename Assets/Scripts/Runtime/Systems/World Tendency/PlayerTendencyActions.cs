using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.WorldTendency
{
    public class PlayerTendencyActions : MonoBehaviour
    {
        [SerializeField] int currentWorldIndex;
        ITendencyServices tendencyServices;

        void Start() => ServiceLocator.Global.Get(out tendencyServices);

        public void OnEnemyDefeated(bool isBoss = false)
        => tendencyServices.GetTendencySystem.AdjustTendency(currentWorldIndex, isBoss ? 0.5f : 0.1f);

        public void OnPlayerDeath() => tendencyServices.GetTendencySystem.AdjustTendency(currentWorldIndex, -0.2f);
    }
}