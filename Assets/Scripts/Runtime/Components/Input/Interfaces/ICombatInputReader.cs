using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface ICombatInputReader
    {
        Vector2 LookDirection { get; }

        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToMovement;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Aim;
        event Action<bool> Attack;
        event Action<bool> AttackHold;
        event Action Reload;
    }
}