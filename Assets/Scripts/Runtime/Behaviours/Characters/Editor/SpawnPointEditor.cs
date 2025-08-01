using UnityEngine;
using UnityEditor;

namespace Universal.Runtime.Behaviours.Characters
{
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmosForSpawnPoint(SpawnPoint spawnPoint, GizmoType gizmoType)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spawnPoint.transform.position, 0.7f);
            Gizmos.DrawLine(spawnPoint.transform.position, spawnPoint.transform.position + spawnPoint.transform.forward * 3f);
        }
    }
}