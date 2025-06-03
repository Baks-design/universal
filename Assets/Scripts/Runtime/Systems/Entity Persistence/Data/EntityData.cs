using System;
using UnityEngine;

namespace Universal.Runtime.Systems.EntityPersistence
{
    [Serializable]
    public class EntityData : ISaveable
    {
        public Vector3 Position;
        public Quaternion Rotation;

        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}