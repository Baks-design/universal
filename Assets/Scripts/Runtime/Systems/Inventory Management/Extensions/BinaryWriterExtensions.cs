﻿using System.IO;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, SerializableGuid guid)
        {
            writer.Write(guid.Part1);
            writer.Write(guid.Part2);
            writer.Write(guid.Part3);
            writer.Write(guid.Part4);
        }
    }
}