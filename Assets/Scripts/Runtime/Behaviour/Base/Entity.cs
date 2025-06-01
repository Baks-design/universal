using UnityEngine;
using Universal.Runtime.Systems.Stats;

namespace Universal.Runtime.Behaviours
{
    public abstract class Entity : MonoBehaviour, IVisitable
    {
        [SerializeField, InLineEditor] BaseStats baseStats;
        
        public Stats Stats { get; private set; }

        void Awake() => Stats = new Stats(new StatsMediator(), baseStats);

        public void Update() => Stats.Mediator.Update();

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}