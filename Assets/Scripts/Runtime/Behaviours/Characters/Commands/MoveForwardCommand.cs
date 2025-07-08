using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveForwardCommand : GridMovementCommand
    {
        public override Vector3 GetMovementDirection(IGridMover mover) => Vector3.forward;
    }
}