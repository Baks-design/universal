using System.IO;
using Universal.Runtime.Systems.EntityPersistence;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public static class BinaryReaderExtensions
    {
        public static SerializableGuid Read(this BinaryReader reader)
        => new(reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32());
    }
}