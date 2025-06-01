using UnityEngine;
using Universal.Runtime.Systems.Stats;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours
{
    public class StatModifierPickup : Pickup
    {
        [SerializeField, InLineEditor] PickupData pickupData;

        protected override void ApplyPickupEffect(Entity entity)
        {
            var modifier = ServiceLocator.For(this)
                .Get<IStatModifierFactory>()
                .Create(pickupData.operatorType, pickupData.type, pickupData.value, pickupData.duration);
            entity.Stats.Mediator.AddModifier(modifier);
        }
    }
}