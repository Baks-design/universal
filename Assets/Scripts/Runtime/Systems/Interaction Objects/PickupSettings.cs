using System;
using UnityEngine;

namespace Universal.Runtime.Systems.InteractionObjects
{
    [Serializable]
    public class PickupSettings
    {
        public LayerMask objectsLayers;
        public float interactionRadius = 0.1f;
        public float interactionRange = 2f;
    }
}