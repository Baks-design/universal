using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IMovementInputReader
    {
        Vector2 MoveDirection { get; }

        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToCombat;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action TurnRight;
        event Action TurnLeft;
    }

    public interface IInvestigateInputReader
    {
        Vector2 LookDirection { get; }

        event Action OpenPauseScreen;
        event Action ToMovement;
        event Action ToCombat;
        event Action AddCharacter;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Aim;
        event Action Interact;
    }

    public interface ICombatInputReader
    {
        Vector2 LookDirection { get; }

        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToCombat;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action Aim;
        event Action Attack;
    }
}