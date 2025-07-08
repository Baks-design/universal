using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface IGridMover
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        float GridSize { get; }
        bool IsMoving { get; }
        bool IsRotating { get; }

        bool TryExecuteCommand(IMovementCommand command);
    }
}