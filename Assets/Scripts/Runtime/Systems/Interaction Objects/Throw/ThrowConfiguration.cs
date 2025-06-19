using System;
using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    [Serializable]
    public class ThrowConfiguration
    {
        public float Force = 100f;
        public float Gravity = Physics.gravity.y;
    }
}