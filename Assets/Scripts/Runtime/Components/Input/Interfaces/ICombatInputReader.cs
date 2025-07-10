using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface ICombatInputReader
    {
        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToMovement;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action<Vector2> Look;
        event Action Aim;
        event Action Attack;
        event Action Target;
        event Action<float> Selection;
    }
}