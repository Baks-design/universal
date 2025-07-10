using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class StrafeLeftCommand : GridMovementCommand
    {
        public override Vector3 GetMovementDirection(IGridMover mover) => mover.Rotation * Vector3.left;

        public override void Execute(IGridMover mover)
        {
            base.Execute(mover);
            HasMove = !HasMove;
        }
    }
}