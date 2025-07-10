using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IGridMover
    {
        bool IsMoving { get; }
        bool IsRotating { get; }
        float GridSize { get; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }

        bool TryExecuteCommand(IMovementCommand command);
    }
}