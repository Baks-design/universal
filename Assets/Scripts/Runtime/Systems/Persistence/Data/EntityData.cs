using System;
using UnityEngine;
using Universal.Runtime.Systems.Persistence.Helpers;
using Universal.Runtime.Systems.Persistence.Interfaces;

namespace Universal.Runtime.Systems.Persistence.Data
{
    [Serializable]
    public class EntityData : ISaveable
    {
        public Vector3 Position;
        public Quaternion Rotation;

        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}