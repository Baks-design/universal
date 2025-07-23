using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public struct InputMovement
    {
        public Vector2 Move;
        public bool IsRunning;

        public readonly bool HasInput => Move != Vector2.zero;
    }
}