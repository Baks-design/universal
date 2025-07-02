using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IInvestigateInputReader
    {
        Vector2 LookDirection { get; }

        event Action OpenPauseScreen;
        event Action ToMovement;
        event Action ToCombat;
        event Action AddCharacter;
        event Action RemoveCharacter;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Aim;
        event Action Interact;
    }
}