using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class MoveForwardCommand : GridMovementCommand
    {
        public override Vector3 GetMovementDirection(IGridMover mover) => mover.Rotation * Vector3.forward;

        public override void Execute(IGridMover mover)
        {
            base.Execute(mover);
            HasMove = !HasMove;
        }
    }
}