using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public class TurnRightCommand : GridTurnCommand
    {
        public override float TurnAngle { get; } = 90f;

        public override void Execute(IGridMover mover)
        {
            base.Execute(mover);
            mover.Rotation *= Quaternion.Euler(0f, TurnAngle, 0f);
        }
    }
}