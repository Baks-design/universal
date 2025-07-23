using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IMovementInputReader
    {
        Vector2 MoveDirection { get; }
        Vector2 LookDirection { get; }

        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToCombat;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Aim;
        event Action<bool> Run;
        event Action Crouch;
        event Action Jump;
    }
}