using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public interface ILifeCurrencyServices
    {
        void TransferCurrency(GameObject from, GameObject to, float amount);
        void AddCurrency(GameObject target, float amount);
        float GetCurrency(GameObject target);
    }
}