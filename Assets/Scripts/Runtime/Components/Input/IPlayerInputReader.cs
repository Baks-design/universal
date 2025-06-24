using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IPlayerInputReader
    {
        Vector2 MoveDirection { get; }
        Vector2 LookDirection { get; }
        float Running { get; }
        float Turning { get; }

        event Action Pause;
        event Action AddCharacter;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Inspection;
        event Action<Vector2, bool> Look;
        event Action Aim;
        event Action Turn;
        event Action<Vector2> Move;
        event Action Run;
        event Action Interact;
        event Action Throw;
    }
}