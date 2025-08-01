using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class SpawnPoint : MonoBehaviour, ISpawnPoint
    {
        public Vector3 Position => transform.localPosition;
        public Quaternion Rotation => transform.localRotation;
        public bool IsAvailable { get; set; } = true;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Position, 0.5f);
            Gizmos.DrawLine(Position, Position + transform.forward * 2f);
        }
    }
}