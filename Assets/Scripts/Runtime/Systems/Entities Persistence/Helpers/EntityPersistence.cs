using Alchemy.Inspector;
using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.EntitiesPersistence
{
    public class EntityPersistence : MonoBehaviour, IBind<EntityData>
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, ReadOnly] EntityData data;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(EntityData data)
        {
            this.data = data;
            this.data.Id = Id;
            tr.SetLocalPositionAndRotation(data.Position, data.Rotation);
        }

        void Update()
        {
            data.Position = tr.localPosition;
            data.Rotation = tr.localRotation;
        }
    }
}