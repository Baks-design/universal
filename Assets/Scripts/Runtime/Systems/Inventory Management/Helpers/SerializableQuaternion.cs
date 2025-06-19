using UnityEngine;
using System;

namespace Universal.Runtime.Systems.InventoryManagement
{
    /// <summary>
    /// Represents a serializable version of the Unity Quaternion struct.
    /// </summary>
    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override readonly string ToString() => $"[{x}, {y}, {z}, {w}]";

        public static implicit operator Quaternion(SerializableQuaternion quaternion)
        => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

        public static implicit operator SerializableQuaternion(Quaternion quaternion)
        => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}