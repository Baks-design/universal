using System;
using System.Collections.Generic;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class LifeCurrencyManager : MonoBehaviour, ILifeCurrencyServices
    {
        readonly Dictionary<GameObject, float> currencyMap = new();

        public event Action<GameObject, float, float> OnCurrencyChanged = delegate { };

        void Awake() => ServiceLocator.Global.Register<ILifeCurrencyServices>(this);

        public void TransferCurrency(GameObject from, GameObject to, float amount)
        {
            if (!SpendCurrency(from, amount)) return;

            AddCurrency(to, amount);
        }

        bool SpendCurrency(GameObject target, float amount)
        {
            if (!currencyMap.ContainsKey(target)) return false;
            if (currencyMap[target] < amount) return false;

            currencyMap[target] -= amount;
            OnCurrencyChanged.Invoke(target, -amount, currencyMap[target]);
            return true;
        }

        public void AddCurrency(GameObject target, float amount)
        {
            if (amount <= 0f) return;

            if (!currencyMap.ContainsKey(target))
                currencyMap[target] = 0f;

            var newAmount = currencyMap[target] + amount;
            currencyMap[target] = newAmount;

            OnCurrencyChanged.Invoke(target, amount, newAmount);
        }

        public float GetCurrency(GameObject target)
        => currencyMap.TryGetValue(target, out var amount) ? amount : 0f;
    }
}