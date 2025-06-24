using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IPlayerInputReader
    {
        Vector2 MoveDirection { get; }
        Vector2 LookDirection { get; }

        event Action Pause;
        event Action AddCharacter;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Inspection;
        event Action<Vector2> Look;
        event Action Aim;
        event Action TurnRight;
        event Action TurnLeft;
        event Action<Vector2> Move;
        event Action Run;
        event Action Interact;
        event Action Throw;
    }
}