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
        event Action Aim;
        event Action TurnRight;
        event Action TurnLeft;
        event Action Run;
        event Action Interact;
        event Action Throw;
        event Action Shoot;
    }
}