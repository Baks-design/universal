using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IMovementInputReader
    {
        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToCombat;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action<Vector2> Look;
        event Action Aim;
        event Action<Vector2> Move;
        event Action<bool> Run;
        event Action Crouch;
        event Action Jump;
    }
}