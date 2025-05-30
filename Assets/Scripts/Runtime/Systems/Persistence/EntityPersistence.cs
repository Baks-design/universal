using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.Persistence.Data;
using Universal.Runtime.Systems.Persistence.Helpers;
using Universal.Runtime.Systems.Persistence.Interfaces;
using Universal.Runtime.Systems.Update;
using Universal.Runtime.Systems.Update.Interface;

namespace Universal.Runtime.Systems.Persistence
{
    public class EntityPersistence : AManagedBehaviour, IUpdatable, IBind<EntityData>
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] EntityData data;

        public EntityData Data => data;
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(EntityData data)
        {
            this.data = data;
            this.data.Id = Id;
            tr.SetLocalPositionAndRotation(data.Position, data.Rotation);
        }

        void IUpdatable.ManagedUpdate()
        {
            data.Position = tr.localPosition;
            data.Rotation = tr.localRotation;
        }
    }
}