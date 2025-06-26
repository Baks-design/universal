using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class HumanoidLimbSystem : MonoBehaviour 
    {
        [SerializeField] LimbSetup[] limbSetups;
        [Serializable]
        class LimbSetup
        {
            public LimbType Type;
            public Transform Transform;
            public float MaxHealth = 100f;
        }
        readonly Dictionary<LimbType, Limb> limbs = new();

        void Awake()
        {
            for (var i = 0; i < limbSetups.Length; i++)
            {
                var setup = limbSetups[i];
                var limb = new Limb(setup.Type, setup.MaxHealth, setup.Transform);
                limbs.Add(setup.Type, limb);
            }
        }

        public Limb[] GetLimbs()
        {
            var array = new Limb[limbs.Count];
            limbs.Values.CopyTo(array, 0);
            return array;
        }

        public Limb GetLimb(LimbType type) => limbs.TryGetValue(type, out var limb) ? limb : null;

        public void RegisterLimb(Limb limb) => limbs[limb.Type] = limb;
    }
}