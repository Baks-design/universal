using UnityEngine;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.WorldTendency
{
    public class TendencyManager : PersistentSingleton<TendencyManager>
    {
        [SerializeField] int worldCount = 5;
        [SerializeField] float[] thresholds = { 0.8f, 0.3f, -0.3f, -0.8f };
        WorldTendency tendencySystem;

        public WorldTendency GetTendencySystem => tendencySystem;

        protected override void Awake()
        {
            base.Awake();

            var calculator = new ThresholdTendencyCalculator(thresholds);
            var persister = new PersisterHandler();

            tendencySystem = new WorldTendency(calculator, persister, worldCount);

            var objectEffect = new ObjectActivationEffect();
            tendencySystem.AddEffect(objectEffect);
        }

        void OnApplicationQuit() => tendencySystem.Save();
    }
}