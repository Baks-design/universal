using System;
using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    [Serializable]
    public class ThrowConfiguration
    {
        [Header("Physics")]
        public float baseForce = 10f;
        public bool useGravity = true;
        public bool allowRotation = false;
        public ForceMode forceMode = ForceMode.Impulse;

        [Header("Advanced")]
        public bool addRandomSpin = false;
        public float spinForce = 1f;
        public float minImpactForce = 2f;
    }
}