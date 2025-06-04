using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.ManagedUpdate;

namespace Universal.Runtime.Systems.EntityPersistence
{
    public class EntityPersistence : AManagedBehaviour, IUpdatable, IBind<EntityData>
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] EntityData data;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(EntityData data)
        {
            this.data = data;
            this.data.Id = Id;
            tr.SetLocalPositionAndRotation(data.Position, data.Rotation);
        }

        void IUpdatable.ManagedUpdate(float deltaTime, float time)
        {
            data.Position = tr.localPosition;
            data.Rotation = tr.localRotation;
        }
    }
}