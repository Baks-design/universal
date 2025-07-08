using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveBackwardCommand : GridMovementCommand
    {
        public override Vector3 GetMovementDirection(IGridMover mover) => Vector3.back;
    }
}