using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.WorldTendency
{
    public class TendencyManager : MonoBehaviour, ITendencyServices
    {
        [SerializeField] int worldCount = 5;
        [SerializeField] float[] thresholds = { 0.8f, 0.3f, -0.3f, -0.8f };
        WorldTendency tendencySystem;

        public WorldTendency GetTendencySystem => tendencySystem;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<ITendencyServices>(this);

            var calculator = new ThresholdTendencyCalculator(thresholds);
            var persister = new PersisterHandler();

            tendencySystem = new WorldTendency(calculator, persister, worldCount);

            var objectEffect = new ObjectActivationEffect();
            tendencySystem.AddEffect(objectEffect);
        }

        void OnApplicationQuit() => tendencySystem.Save(); 
    }
}